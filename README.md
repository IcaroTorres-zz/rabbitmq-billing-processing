# RabbitMq-Billing-Processing

[![ci-build](https://github.com/IcaroTorres/rabbitmq-billing-processing/actions/workflows/ci-build.yml/badge.svg?branch=main)](https://github.com/IcaroTorres/rabbitmq-billing-processing/actions/workflows/ci-build.yml)
[![ci-sonarcloud](https://github.com/IcaroTorres/rabbitmq-billing-processing/actions/workflows/ci-sonar.yml/badge.svg?branch=main)](https://github.com/IcaroTorres/rabbitmq-billing-processing/actions/workflows/ci-sonar.yml)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=security_rating)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)

[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=code_smells)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=bugs)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=IcaroTorres_rabbitmq-billing-processing&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=IcaroTorres_rabbitmq-billing-processing)

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


## Disclaimer

> For the sake of simplicity, all applications here were written as 'One Class Library Project' due to containing many of them together per solution for demonstration purposes. But the layers are split and often represented by the folders 'Application', 'Infrastructure', and 'Domain', with the Presentation as the project Itself.

## Scenario

### Customers API

> Service that allows registration and consultation of customers.
- The registration method must receive as parameters: Name (string), State (string), CPF (string), must validate the CPF and format it to a numerical value, in addition, duplicate CPFs or empty fields must not be accepted. Persist the data the way you want.
- The Query Method must receive a CPF (string) as parameters, validate the CPF and perform the query.

#### Endpoints

> POST - /api/customers
request body: 
```json
{
    "cpf": "97538515615",
    "name": "sample user name",
    "state": "MG"
}
``` 

- 201 Created response body: (with location in header)
```json
{
  "data": {
    "cpf": 97538515615,
    "name": "SAMPLE USER NAME",
    "state": "MG"
  },
  "errors": []
}
```

- 409 Conflict response body:
```json
{
  "data": null,
  "errors": ["Cliente já existe para dado Cpf 97538515615"]
}
```

- 400 BadRequest response body:
```json
{
  "data": null,
  "errors": [
    "Nome não pode ser vazio ou nulo",
    "Estado inválido",
    "Cpf inválido"
  ]
}
```

> GET - /api/customers/{cpf}

- 200 OK response body:
```json
{
  "data": {
    "cpf": 97538515615,
    "name": "SAMPLE USER NAME",
    "state": "MG"
  },
  "errors": []
}
```

- 400 OK response body:
```json
{
  "data": null,
  "errors": ["Cpf inválido"]
}
```

- 404 NotFound response body:
```json
{
  "data": null,
  "errors": ["Cliente não encontrado para dado Cpf 97538515615"]
}
```

### Issuance API

> Service that records a charge for a particular customer.
- The API should expose a method that takes as parameters the Due Date, CPF and the amount of the billing. After validating the data, the API must persist the charges received in MongoDB.
- The API must expose a method that receives a CPF or a reference month as a parameter and returns the charges registered according to the filter (CPF, Due Date and Amount). At least one of the filters is mandatory.

#### Endpoints

> POST - /api/billings
request body: 
```json
{
    "cpf": "97538515615",
    "amount": 123.56,
    "dueDate": "20-07-2021"
}
``` 

- 201 Created response body: (without location in header due to lack of unique get endpoint)
```json
{
  "data": {
    "id": "4ada6a08-4a8d-4c0d-82f3-2ae3cd4ddd6a",
    "cpf": "97538515615",
    "dueDate": "20-07-2021",
    "amount": 123.56
  },
  "errors": []
}
```

- 400 BadRequest response body:
```json
{
  "data": null,
  "errors": [
    "Vencimento precisa representar uma data válida futura no formato [dd-MM-yyyy]",
    "Valor não pode ser 0 ou negativo",
    "Cpf inválido"
  ]
}
```

> GET - api/billings?month=06-2021&cpf=02903084530

- 200 OK response body:
```json
{
  "data": [{
    "id": "4ada6a08-4a8d-4c0d-82f3-2ae3cd4ddd6a",
    "cpf": "97538515615",
    "dueDate": "20-07-2021",
    "amount": 123.56
  }],
  "errors": []
}
```

- 400 OK response body:
```json
{
  "data": null,
  "errors": [
    "Cpf inválido",
    "Informe um valor para ao menos um dos dois Cpf e/ou mês",
    "Vencimento precisa atender o formato [MM-yyyy], com mês de 1 a 12 e ano >= 2000"
  ]
}
```

### Billing Processing Service

> Service that calculates the value of customer charges.
- Its process consists of consulting all registered customers, calculating and registering charges as quickly as possible (Using the two APIs built in the previous steps). The calculation is made as follows: first 2 digits of the CPF concatenated to the last 2 digits of the customer's CPF. For example, on CPF 12345678, the amount charged will be R$ 1278.00.

#### Endpoints

> Don't exposes end user APIs

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

#### Architecture

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

#### Library

This projects simulates Enterprise Private packages available for shared development libraries.
For the sake of lazyness leaving me from code repetition among the services :)


### Eventual Billing Processing (developing)

This solution proposes:

- 2 Web APIs (Customers, Issuance) interacting with end-users;
- performing Its independent use cases (create and query);
- persisting their data in each respective database;
- but emitting events notifying Its entities creations;
- to a third Pub / Sub processing service consuming and reacting to them;
- partially and temporarilly replicating data to control the event flow; 
- in case of billings arriving before respective customer;
- they are stored grouped by customer cpf key as awaiting Its arrival;
- being processed when both customer and billings data are fetch togheter;
- them sending processed event to Its queue;
- releasing temporary data when the message become acked.

#### Architecture

![Billing Processing - Scheduled Architecture](https://github.com/icarotorres/rabbitmq-billing-processing/blob/main/EventualProcessing/billing-processing-eventual-architecture.png?raw=true)

#### Web APIs

As related before with addition of:

- Application services instead of Hosted services handling mesages;
- pub and sub handling integrated to mediator pipelines;
- possibility of reusing event handling use cases exposing then to public endpoints for manual iteraction if needed.
```
{
  "data" : {},
  "errors": [string]
}
```

#### EventualProcessing.Worker

Similar to ScheduledProcessing.Worker project definition except by being a Pub / Sub AMQP Worker using events instead of Procedure Calls.

- contains simplified model representions of customers and billing entities;
- consumes both 'customer_registered' and 'billing_issued' events and react to them;
- controlls the event flow persisting data partially replicated from APIs;
- discarding them as soon as the flow is confirmed;
- when both counterparts are received, runs all billings in parallel;
- with no need of mathing related customer by binary searchs;
- by having It previously persisted by key or being imediately received;
- and call implemented algorithm to process the pairs and emit a batch_processed event when finished;
- performing a per-customer processing but allowing multiple customers being processed by different Consumer instances;

#### Library

Almos the same of the Scheduled version but bigger with shared implementations sharing event handling code
easing pub / sub integrations with previously made API request/response pipeline use cases. 
