namespace FsPolyJobs.Domain

open Validus
open System

module internal Utils =
    let trim value =
        if String.IsNullOrWhiteSpace value then
            ""
        else
            value.Trim()

open Utils

type JobTitle =
    { value: string }
    static member Create: Validator<string, JobTitle> =
        fun field input ->
            validate {
                let input = trim input
                let! input = input |> Check.String.notEmpty field
                let! input = input |> Check.String.lessThanLen 101 field
                return { value = input }
            }

type JobDescription =
    { value: string option }
    static member Create: Validator<string, JobDescription> =
        fun field input ->
            validate {
                let input = trim input

                if String.IsNullOrEmpty input then
                    return { value = None }
                else
                    let! input = input |> Check.String.lessThanLen 0x1001 field
                    return { value = Some input }
            }

type JobType =
    { value: string }
    static member Create: Validator<string, JobType> =
        fun field input ->
            validate {
                let input = trim input
                let! input = input |> Check.String.notEmpty field
                let! input = input |> Check.String.lessThanLen 41 field
                return { value = input }
            }

type JobUserData =
    { value: string option }
    static member Create: Validator<string, JobUserData> =
        fun field input ->
            validate {
                if isNull input then
                    return { value = None }
                else
                    let! input = input |> Check.String.lessThanLen 0x10001 field
                    return { value = Some input }
            }

type JobStatus =
    | InProgress
    | Success of JobUserData
    | Failure of JobUserData
    static member Create: Validator<JobStatusInput, JobStatus> =
        fun field input ->
            validate {
                match input with
                | InProgressInput -> return InProgress
                | SuccessInput input ->
                    let! input = input |> JobUserData.Create(field + ".success")
                    return Success input
                | FailureInput input ->
                    let! input = input |> JobUserData.Create(field + ".failure")
                    return Failure input
            }

type JobProgress =
    { value: int }
    static member Create: Validator<int option, JobProgress> =
        fun field input ->
            validate {
                match input with
                | None -> return { value = 0 }
                | Some input ->
                    let! input = input |> Check.Int.greaterThan -1 field
                    return { value = input }
            }

    static member CreateOptional: Validator<int option, JobProgress option> =
        fun field input ->
            validate {
                match input with
                | None -> return None
                | Some input ->
                    let! input = input |> Check.Int.greaterThan -1 field
                    return Some { value = input }
            }

type JobMaxProgress =
    { value: int }
    static member Create: Validator<int option, JobMaxProgress> =
        fun field input ->
            validate {
                match input with
                | None -> return { value = 100 }
                | Some input ->
                    let! input = input |> Check.Int.greaterThan 0 field
                    return { value = input }
            }

    static member CreateOptional: Validator<int option, JobMaxProgress option> =
        fun field input ->
            validate {
                match input with
                | None -> return None
                | Some input ->
                    let! input = input |> Check.Int.greaterThan 0 field
                    return Some { value = input }
            }

type JobMetadata =
    { value: Map<string, string> }
    static member Create: Validator<Map<string, string> option, JobMetadata> =
        let validateNoEmptyKeys =
            Validator.create (sprintf "'%s' keys cannot be empty") (fun input ->
                input
                |> Map.forall (fun key _ -> not (String.IsNullOrWhiteSpace key)))

        let validateNoEmptyValues =
            Validator.create (sprintf "'%s' values cannot be empty") (fun input ->
                input
                |> Map.forall (fun _ value -> not (String.IsNullOrWhiteSpace value)))

        fun field input ->
            validate {
                match input with
                | None -> return { value = Map.empty }
                | Some input ->
                    let! _ = input |> validateNoEmptyKeys field
                    and! _ = input |> validateNoEmptyValues field
                    return { value = input }
            }
