[<AutoOpen>]
module FsPolyJobs.Domain.JobTestsPreamble

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
