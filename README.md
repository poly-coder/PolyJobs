# PolyJobs

Implementation of a job repository on multiple languages.

## Languages

- [.] F#
- [ ] C#
- [ ] Go
- [ ] Rust
- [ ] Elixir
- [ ] Clojure
- [ ] Racket
- [ ] Scala
- [ ] Python

## Domain

- [ ] Start a job with a state
- [ ] Update job progress and state
- [ ] Complete job with failure or success

## APIs

- [ ] REST
- [ ] NATS
- [ ] gRPC
- [ ] GraphQL
- [ ] AMQP
- [ ] TCP
- [ ] ZeroMQ

## Persistence

- [.] In-memory
- [ ] Redis
- [ ] Event Sourcing on NATS JetStream
- [ ] SQL
- [ ] noSQL

## Extensions

- [ ] Validation
- [ ] Actor model
- [ ] Domain Driven Design
- [ ] Telemetry
- [ ] Dockerfile
- [ ] Unit Testing
- [ ] Integration Testing
- [ ] I18n

## Table of development

| Features            | F#  | C#  | Go  | Rust | Elixir | Clojure | Racket | Scala | Python |
| ------------------- | --- | --- | --- | ---- | ------ | ------- | ------ | ----- | ------ |
| Domain              |     |     |     |      |        |         |        |       |        |
| Validation          | ✅   |     |     |      |        |         |        |       |        |
| Actor Model         |     |     |     |      |        |         |        |       |        |
| Telemetry           |     |     |     |      |        |         |        |       |        |
| Dockerfile          |     |     |     |      |        |         |        |       |        |
| Unit Testing        |     |     |     |      |        |         |        |       |        |
| Integration Testing |     |     |     |      |        |         |        |       |        |
| I18n                |     |     |     |      |        |         |        |       |        |
| REST                |     |     |     |      |        |         |        |       |        |
| NATS                |     |     |     |      |        |         |        |       |        |
| gRPC                |     |     |     |      |        |         |        |       |        |
| GraphQL             |     |     |     |      |        |         |        |       |        |
| AMQP                |     |     |     |      |        |         |        |       |        |
| TCP                 |     |     |     |      |        |         |        |       |        |
| ZeroMQ              |     |     |     |      |        |         |        |       |        |
| In-memory           |     |     |     |      |        |         |        |       |        |
| Redis               |     |     |     |      |        |         |        |       |        |
| Event Sourcing      |     |     |     |      |        |         |        |       |        |
| SQL                 |     |     |     |      |        |         |        |       |        |
| noSQL               |     |     |     |      |        |         |        |       |        |

### F#

Implementation of Jobs service in F#.

#### Domain

#### Validation

Validation can be performed using the abstraction `Result<'T, 'TErrors>`.

There are multiple libraries that can be used to perform validation:

- [FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling)
  Is a library that provide a lots of CE for working with `Option<'a>`, `Result<'a, 'e>`, `Async<'a>` and `Task<'a>`.

  It also provide some CEs for combinations of the previous, like `Result<'a option, 'e>`, `Async<Result<'a, 'e>>` and `Task<Result<'a, 'e>>`. Also, the more advanced `Async<Result<'a option, 'e>>` and `Result<'a, 'e list>`.

  It lacks structure for validation, like having property path and error message, but it can be very useful for simple validation and using their CEs.

- [Validus](https://github.com/pimbrouwers/Validus)
  Is a library that provide a structure for validation, like having property path and error message.

  Its main type definition is `ValidationResult<'a> = Result<'a, ValidationErrors>`, where `ValidationErrors` represents a map of property paths to error messages list.

  It also contains a `Validator` type, a `validate` CE and a `Check` module with some prebuilt validations.

  It lacks support for async validations, but if necessary, it can be implemented using the `AsyncResult` type from `FsToolkit.ErrorHandling`.

- [FSharp.Control.FusionTasks](https://github.com/kekyo/FSharp.Control.FusionTasks)
  It is a library that provide some extensions to `Async` and `Task` workflows, to use them interchangeably.
