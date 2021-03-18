# RabbitMq-Billing-Processing

## Description

In this repo I'll demonstrate different approaches to solving a fictitious Customer Billing Processing scenario.
For such end are used .Net core 3.1, RabbitMQ, MongoDB Atlas, Redis Distributed Cache, Sqlite, Rest APIs and Console Workers.

## Purpose

The solutions are made with the intention of experimenting and practicing techniques used in the composition of microservices architectures,
without relying on paid licenses, hardware and software installation.

## Requirements

 - .Net Core 3.1
 - A compatible IDE
 - Internet (as all services used are cloud-based).
 
 This repo was designed to have only self-contained solutions, executable without installations, except for the nuget packages used.

## Stack applied

- Messaging service - ![CloudAMQP](https://www.cloudamqp.com/)
- NoSql Database - ![MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
- NoSql Distributed Cache - ![RedisCloud](https://redislabs.com/redis-enterprise-cloud/overview/)
- Sql Database (generated through nuget packages and via migrations) - ![Sqlite](https://www.sqlite.org/)


## Scenario

### Customers API

> Service that allows registration and consultation of customers.
- The registration method must receive as parameters: Name (string), State (string), CPF (string), must validate the CPF and format it to a numerical value, in addition, duplicate CPFs or empty fields must not be accepted. Persist the data the way you want.
- The Query Method must receive a CPF (string) as parameters, validate the CPF and perform the query.

### Issuance API

> Service that records a charge for a particular customer.
- The API should expose a method that takes as parameters the Expiration Date, CPF and the amount of the charge. After validating the data, the API must persist the charges received in MongoDB.
- The API must expose a method that receives a CPF or a reference month as a parameter and returns the charges registered according to the filter (CPF, Due Date and Amount). At least one of the filters is mandatory.

### Billing Processing Service

> Service that calculates the value of customer charges.
- Its process consists of consulting all registered customers, calculating and registering charges as quickly as possible (Using the two APIs built in the previous steps). The calculation is made as follows: first 2 digits of the CPF concatenated to the last 2 digits of the customer's CPF. For example, on CPF 12345678, the amount charged will be R$ 1278.00.

## Solutions

### Scheduled Billing Processing (developed)

This solution proposes:

- 2 isollated Web APIs (Customers, Issuance) interacting with end-users;
- performing Its independent use cases (create and query);
- persisting their data in each respective database without any data replication between them;
- the processing is executed by a third Worker Service as a scheduled batch of changes;
- done by Remote Procedure Calls from iT being a RPC client;
- to both APIs having background hosted services implemented as RPC servers;
- receiving changes and returning current data to be processed.

#### Architectire

![Billing Processing - Scheduled Architecture](https://github.com/icarotorres/rabbitmq-billing-processing/blob/main/ScheduledProcessing/billing-processing-scheduled-architecture.png?raw=true)

#### Web APIs

Both Customers and Issuance APIs operates in a similar manner:

- http requests accepting and returning json content;
- rest (hateoas left behind for simplicity);
- swagger docs;
- mediatR request/response pipeline with fluent validation behavior;
- fail fast with lighter queries for pre-conditions before executing acctual use cases;
- default response contract as following:
```
{
  "data" : {},
  "errors": [string]
}
```

#### ScheduledProcessing.Worker

A Web hosted console application, instead of a normal console. Making it easier to monitor and collect
metrics for the execution and health of the application if needed, in addition to benefiting from the
easy dependency injection of the web host.

- contains simplified model representions of customers and billing entities;
- scheduled delay between batchs configured from appsettings allowing diferent intervals for each release;
- do remote procedure calls to servers on each apis in separated tasks and wait for both;
- as soon as both counterparts are received, runs all billings in parallel;
- matching its related customer by binary searchs;
- and call implemented algorithm to process the pairs and send the batch in the next scheduled time (as short as possible);
- if the processed batch failed to be received on the rpc server, it will be remade automatically on next time;
- because the server service will not update the records and will resend them in the next requested batch;
- so when the RPC client and both servers are running.

- 

#### Library

This projects simulates Enterprise Private packages available for shared development libraries.
For the sake of lazyness leaving me from code repetition among the services :)
