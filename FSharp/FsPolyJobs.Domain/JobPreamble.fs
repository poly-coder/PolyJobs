module FsPolyJobs.Domain.JobPreamble

open System

let trim value =
    if String.IsNullOrWhiteSpace value then
        ""
    else
        value.Trim()

let addMap map1 map2 =
    map1
    |> Map.toSeq
    |> Seq.append (map2 |> Map.toSeq)
    |> Map.ofSeq
        