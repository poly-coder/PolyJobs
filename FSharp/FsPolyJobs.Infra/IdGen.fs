namespace FsPolyJobs.Infra

open FsPolyJobs.App
open System

type GuidIdGen() =
    interface IIdGen with
        member this.NewId = Guid.NewGuid().ToString("N")
