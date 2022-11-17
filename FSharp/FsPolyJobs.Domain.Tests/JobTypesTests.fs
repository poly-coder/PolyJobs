module FsPolyJobs.Domain.JobTypesTests

open System
open Xunit
open Swensen.Unquote
open Validus

let toErrorList (errors: string) =
    if isNull errors then
        []
    else
        errors.Split(';')
        |> Seq.map (fun s -> s.Trim())
        |> Seq.filter (String.IsNullOrEmpty >> not)
        |> Seq.toList

let toMetadataMap (pairs: string) =
    if String.IsNullOrWhiteSpace pairs then
        Seq.empty
    else
        let clean s = if s = "" then null else s

        pairs.Split(';')
        |> Seq.map (fun s ->
            match s.Split('=') with
            | [| a; b |] -> clean a, clean b
            | _ -> failwith "Invalid pair")
    |> Map.ofSeq

[<Theory>]
[<InlineData("Some title", "Some title")>]
[<InlineData("X", "X")>]
[<InlineData("  Trimmable title   \t\r\n ", "Trimmable title")>]
let ``JobTitle.Create valid`` (input: string) (expectedValue: string) =
    let result = input |> JobTitle.Create "title"
    let expected: JobTitle = { value = expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(null, "'title' must not be empty")>]
[<InlineData("", "'title' must not be empty")>]
[<InlineData("    ", "'title' must not be empty")>]
[<InlineData("A very long title that should be rejected because only shorter titles are valid, never beyond 100 characters.",
             "'title' must be at less than 101 characters")>]
let ``JobTitle.Create errors`` (input: string) (errors: string) =
    let result = input |> JobTitle.Create "title"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "title"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("", null)>]
[<InlineData("    ", null)>]
[<InlineData("Some description", "Some description")>]
[<InlineData("X", "X")>]
[<InlineData("  Trimmable description   \t\r\n ", "Trimmable description")>]
let ``JobDescription.Create valid`` (input: string) (expectedValue: string) =
    let result =
        input |> JobDescription.Create "description"

    let expected: JobDescription = { value = Option.ofObj expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData("Some jobType", "Some jobType")>]
[<InlineData("X", "X")>]
[<InlineData("  Trimmable jobType   \t\r\n ", "Trimmable jobType")>]
let ``JobType.Create valid`` (input: string) (expectedValue: string) =
    let result = input |> JobType.Create "jobType"
    let expected: JobType = { value = expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(null, "'jobType' must not be empty")>]
[<InlineData("", "'jobType' must not be empty")>]
[<InlineData("    ", "'jobType' must not be empty")>]
[<InlineData("A very long jobType that should be rejected", "'jobType' must be at less than 41 characters")>]
let ``JobType.Create errors`` (input: string) (errors: string) =
    let result = input |> JobType.Create "jobType"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "jobType"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("", "")>]
[<InlineData("    ", "    ")>]
[<InlineData("Some payload", "Some payload")>]
[<InlineData("X", "X")>]
[<InlineData("  Trimmable payload   \t\r\n ", "  Trimmable payload   \t\r\n ")>]
let ``JobUserData.Create valid`` (input: string) (expectedValue: string) =
    let result = input |> JobUserData.Create "payload"
    let expected: JobUserData = { value = Option.ofObj expectedValue }
    test <@ result = Ok expected @>

[<Fact>]
let ``JobStatus.Create valid on InProgressInput`` () =
    let input = InProgressInput
    let result = input |> JobStatus.Create "status"
    let expected = InProgress
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("Some status", "Some status")>]
let ``JobStatus.Create valid on SuccessInput`` (input: string) (expectedValue: string) =
    let input = SuccessInput input
    let result = input |> JobStatus.Create "status"

    let expected =
        Success { value = Option.ofObj expectedValue }

    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("Some status", "Some status")>]
let ``JobStatus.Create valid on FailureInput`` (input: string) (expectedValue: string) =
    let input = FailureInput input
    let result = input |> JobStatus.Create "status"

    let expected =
        Failure { value = Option.ofObj expectedValue }

    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(null, 0)>]
[<InlineData(0, 0)>]
[<InlineData(10, 10)>]
[<InlineData(100, 100)>]
[<InlineData(1000, 1000)>]
let ``JobProgress.Create valid`` (input: int option) (expectedValue: int) =
    let result = input |> JobProgress.Create "progress"
    let expected: JobProgress = { value = expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(-1, "'progress' must be greater than -1")>]
[<InlineData(-100, "'progress' must be greater than -1")>]
let ``JobProgress.Create errors`` (input: int) (errors: string) =
    let result =
        Some input |> JobProgress.Create "progress"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "progress"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData(0, 0)>]
[<InlineData(10, 10)>]
[<InlineData(100, 100)>]
[<InlineData(1000, 1000)>]
let ``JobProgress.CreateOptional valid`` (input: int option) (expectedValue: int option) =
    let result =
        input |> JobProgress.CreateOptional "progress"

    let expected: JobProgress option =
        expectedValue
        |> Option.map (fun x -> { value = x })

    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(-1, "'progress' must be greater than -1")>]
[<InlineData(-100, "'progress' must be greater than -1")>]
let ``JobProgress.CreateOptional errors`` (input: int) (errors: string) =
    let result =
        Some input
        |> JobProgress.CreateOptional "progress"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "progress"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null, 100)>]
[<InlineData(1, 1)>]
[<InlineData(10, 10)>]
[<InlineData(100, 100)>]
[<InlineData(1000, 1000)>]
let ``JobMaxProgress.Create valid`` (input: int option) (expectedValue: int) =
    let result =
        input |> JobMaxProgress.Create "maxProgress"

    let expected: JobMaxProgress = { value = expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(0, "'maxProgress' must be greater than 0")>]
[<InlineData(-100, "'maxProgress' must be greater than 0")>]
let ``JobMaxProgress.Create errors`` (input: int) (errors: string) =
    let result =
        Some input |> JobMaxProgress.Create "maxProgress"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "maxProgress"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData(1, 1)>]
[<InlineData(10, 10)>]
[<InlineData(100, 100)>]
[<InlineData(1000, 1000)>]
let ``JobMaxProgress.CreateOptional valid`` (input: int option) (expectedValue: int option) =
    let result =
        input
        |> JobMaxProgress.CreateOptional "maxProgress"

    let expected: JobMaxProgress option =
        expectedValue
        |> Option.map (fun x -> { value = x })

    test <@ result = Ok expected @>

[<Theory>]
[<InlineData(0, "'maxProgress' must be greater than 0")>]
[<InlineData(-100, "'maxProgress' must be greater than 0")>]
let ``JobMaxProgress.CreateOptional errors`` (input: int) (errors: string) =
    let result =
        Some input
        |> JobMaxProgress.CreateOptional "maxProgress"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "maxProgress"

    test <@ result = Error expected @>

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("a=A;b=B;c=C")>]
let ``JobMetadata.Create valid`` (input: string) =
    let input =
        input |> Option.ofObj |> Option.map toMetadataMap

    let expectedValue = input |> Option.defaultValue Map.empty
    let result = input |> JobMetadata.Create "meta"
    let expected: JobMetadata = { value = expectedValue }
    test <@ result = Ok expected @>

[<Theory>]
[<InlineData("a=;b=", "'meta' values cannot be empty")>]
[<InlineData("=A;=B", "'meta' keys cannot be empty")>]
[<InlineData("=;=", "'meta' keys cannot be empty;'meta' values cannot be empty")>]
let ``JobMetadata.Create errors`` (input: string) (errors: string) =
    let input =
        input |> Option.ofObj |> Option.map toMetadataMap

    let result = input |> JobMetadata.Create "meta"

    let expected =
        toErrorList errors
        |> ValidationErrors.create "meta"

    test <@ result = Error expected @>
