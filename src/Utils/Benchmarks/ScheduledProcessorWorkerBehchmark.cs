using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Processing.Scheduled.Worker.Workers;
using UnitTests.Scheduled.Worker.Helpers;

namespace Benchmarks
{
  [MemoryDiagnoser]
  [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
  [RankColumn]
  public class ScheduledProcessorWorkerBehchmark
  {
    [Params(25)] public int CustomersCount;
    [Params(25)]  public int BillingsPerCustomerCount;
    private ScheduledProcessorWorker _worker;
    private List<ICpfCarrier> _customers;
    private ProcessBatch _batch;

    [GlobalSetup]
    public void GlobalSetup()
    {
      _worker = new ScheduledProcessorWorker(
        default, default, new MathOnlyAmountProcessor(), new CpfCarrierComparer(), default, default);
    }

    [IterationSetup]
    public void IterationSetup()
    {
      _customers = new List<ICpfCarrier>(InternalFakes.Customers.Valid().Generate(CustomersCount));
      _batch = new ProcessBatch
      {
        Customers = _customers,
        Billings = _customers
          .SelectMany(x =>
            InternalFakes.Billings
              .Valid(x.Cpf)
              .Generate(BillingsPerCustomerCount))
          .ToList()
      };
    }
    [IterationCleanup]
    public void IterationCleanup()
    {
      _customers = null;
      _batch = null;
    }

    [Benchmark]
    public ProcessBatch LinsIndexes()
    {
      return _worker.ProcessBatch(_batch);
    }

    [Benchmark]
    public ProcessBatch Join()
    {
      return _worker.ProcessBatchJoin(_batch);
    }

    [Benchmark]
    public ProcessBatch GroupJoin()
    {
      return _worker.ProcessBatchGroupJoin(_batch);
    }

    [Benchmark]
    public ProcessBatch JoinGroupSelectMany()
    {
      return _worker.ProcessBatchJoinGroupSelectMany(_batch);
    }
  }
}
