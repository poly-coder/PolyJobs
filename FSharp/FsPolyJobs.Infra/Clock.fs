namespace FsPolyJobs.Infra

open FsPolyJobs.App
open System

type SystemClock() =
    interface IClock with
        member this.UtcNow = DateTime.UtcNow
