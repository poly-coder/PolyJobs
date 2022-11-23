namespace FsPolyJobs.App

open FsPolyJobs.Domain
open FsToolkit.ErrorHandling
open System

type IClock =
    abstract member UtcNow : DateTime

type IIdGen =
    abstract member NewId : string
