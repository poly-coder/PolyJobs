module FsPolyJobs.Domain.JobCommandsTests

open System
open Xunit
open Swensen.Unquote
open Validus

[<Fact>]
let ``JobCreate.Create valid`` () =
    let input: JobCreateInput =
        { title = "title"
          description = "description"
          jobType = "jobType"
          stepType = "stepType"
          payload = "payload"
          progress = Some 1
          maxProgress = Some 2
          status = SuccessInput "status"
          meta = Some(toMetadataMap "a=b;c=d") }

    let result = input |> JobCreate.Create "create"

    let expected: JobCreate =
        { title = { value = input.title }
          description = { value = Some input.description }
          jobType = { value = input.jobType }
          stepType = { value = input.stepType }
          payload = { value = Some input.payload }
          progress = { value = input.progress.Value }
          maxProgress = { value = input.maxProgress.Value }
          status = Success { value = Some "status" }
          meta = { value = input.meta.Value } }

    test <@ result = Ok expected @>

[<Fact>]
let ``JobCreate.Create errors`` () =
    let input: JobCreateInput =
        { title = null
          description = null
          jobType = null
          stepType = null
          payload = null
          progress = Some -1
          maxProgress = Some -2
          status = SuccessInput null
          meta = Some(toMetadataMap "=") }

    let result = input |> JobCreate.Create "create"

    let expected =
        ValidationErrors.collect [ ValidationErrors.create "create.jobType" [ "'create.jobType' must not be empty" ]
                                   ValidationErrors.create
                                       "create.maxProgress"
                                       [ "'create.maxProgress' must be greater than 0" ]
                                   ValidationErrors.create
                                       "create.meta"
                                       [ "'create.meta' keys cannot be empty"
                                         "'create.meta' values cannot be empty" ]
                                   ValidationErrors.create
                                       "create.progress"
                                       [ "'create.progress' must be greater than -1" ]
                                   ValidationErrors.create "create.stepType" [ "'create.stepType' must not be empty" ]
                                   ValidationErrors.create "create.title" [ "'create.title' must not be empty" ] ]

    test <@ result = Error expected @>

[<Fact>]
let ``JobUpdate.Create valid`` () =
    let input: JobUpdateInput =
        { stepType = "stepType"
          payload = "payload"
          progress = Some 1
          maxProgress = Some 2
          status = SuccessInput "status"
          meta = Some(toMetadataMap "a=b;c=d") }

    let result = input |> JobUpdate.Create "update"

    let expected: JobUpdate =
        { stepType = { value = input.stepType }
          payload = { value = Some input.payload }
          progress =
            input.progress
            |> Option.map (fun v -> { value = v })
          maxProgress =
            input.maxProgress
            |> Option.map (fun v -> { value = v })
          status = Success { value = Some "status" }
          meta = { value = input.meta.Value } }

    test <@ result = Ok expected @>

[<Fact>]
let ``JobUpdate.Create errors`` () =
    let input: JobUpdateInput =
        { stepType = null
          payload = null
          progress = Some -1
          maxProgress = Some -2
          status = SuccessInput null
          meta = Some(toMetadataMap "=") }

    let result = input |> JobUpdate.Create "update"

    let expected =
        ValidationErrors.collect [ ValidationErrors.create
                                       "update.maxProgress"
                                       [ "'update.maxProgress' must be greater than 0" ]
                                   ValidationErrors.create
                                       "update.meta"
                                       [ "'update.meta' keys cannot be empty"
                                         "'update.meta' values cannot be empty" ]
                                   ValidationErrors.create
                                       "update.progress"
                                       [ "'update.progress' must be greater than -1" ]
                                   ValidationErrors.create "update.stepType" [ "'update.stepType' must not be empty" ] ]

    test <@ result = Error expected @>

[<Fact>]
let ``JobCommand.Create valid JobCreate`` () =
    let input: JobCreateInput =
        { title = "title"
          description = "description"
          jobType = "jobType"
          stepType = "stepType"
          payload = "payload"
          progress = Some 1
          maxProgress = Some 2
          status = SuccessInput "status"
          meta = Some(toMetadataMap "a=b;c=d") }

    let cmdInput = JobCreateInput input

    let result = cmdInput |> JobCommand.Create "command"

    let expected: JobCreate =
        { title = { value = input.title }
          description = { value = Some input.description }
          jobType = { value = input.jobType }
          stepType = { value = input.stepType }
          payload = { value = Some input.payload }
          progress = { value = input.progress.Value }
          maxProgress = { value = input.maxProgress.Value }
          status = Success { value = Some "status" }
          meta = { value = input.meta.Value } }

    let cmdExpected = JobCreate expected

    test <@ result = Ok cmdExpected @>

[<Fact>]
let ``JobCommand.Create errors JobCreate`` () =
    let input: JobCreateInput =
        { title = null
          description = null
          jobType = null
          stepType = null
          payload = null
          progress = Some -1
          maxProgress = Some -2
          status = SuccessInput null
          meta = Some(toMetadataMap "=") }

    let cmdInput = JobCreateInput input

    let result = cmdInput |> JobCommand.Create "command"

    let expected =
        ValidationErrors.collect [ ValidationErrors.create "command.create.jobType" [ "'command.create.jobType' must not be empty" ]
                                   ValidationErrors.create
                                       "command.create.maxProgress"
                                       [ "'command.create.maxProgress' must be greater than 0" ]
                                   ValidationErrors.create
                                       "command.create.meta"
                                       [ "'command.create.meta' keys cannot be empty"
                                         "'command.create.meta' values cannot be empty" ]
                                   ValidationErrors.create
                                       "command.create.progress"
                                       [ "'command.create.progress' must be greater than -1" ]
                                   ValidationErrors.create "command.create.stepType" [ "'command.create.stepType' must not be empty" ]
                                   ValidationErrors.create "command.create.title" [ "'command.create.title' must not be empty" ] ]

    test <@ result = Error expected @>

[<Fact>]
let ``JobCommand.Create valid JobUpdate`` () =
    let input: JobUpdateInput =
        { stepType = "stepType"
          payload = "payload"
          progress = Some 1
          maxProgress = Some 2
          status = SuccessInput "status"
          meta = Some(toMetadataMap "a=b;c=d") }

    let cmdInput = JobUpdateInput input

    let result = cmdInput |> JobCommand.Create "command"

    let expected: JobUpdate =
        { stepType = { value = input.stepType }
          payload = { value = Some input.payload }
          progress =
            input.progress
            |> Option.map (fun v -> { value = v })
          maxProgress =
            input.maxProgress
            |> Option.map (fun v -> { value = v })
          status = Success { value = Some "status" }
          meta = { value = input.meta.Value } }

    let cmdExpected = JobUpdate expected

    test <@ result = Ok cmdExpected @>

[<Fact>]
let ``JobCommand.Create errors JobUpdate`` () =
    let input: JobUpdateInput =
        { stepType = null
          payload = null
          progress = Some -1
          maxProgress = Some -2
          status = SuccessInput null
          meta = Some(toMetadataMap "=") }

    let cmdInput = JobUpdateInput input

    let result = cmdInput |> JobCommand.Create "command"

    let expected =
        ValidationErrors.collect [ ValidationErrors.create
                                       "command.update.maxProgress"
                                       [ "'command.update.maxProgress' must be greater than 0" ]
                                   ValidationErrors.create
                                       "command.update.meta"
                                       [ "'command.update.meta' keys cannot be empty"
                                         "'command.update.meta' values cannot be empty" ]
                                   ValidationErrors.create
                                       "command.update.progress"
                                       [ "'command.update.progress' must be greater than -1" ]
                                   ValidationErrors.create "command.update.stepType" [ "'command.update.stepType' must not be empty" ] ]

    test <@ result = Error expected @>
