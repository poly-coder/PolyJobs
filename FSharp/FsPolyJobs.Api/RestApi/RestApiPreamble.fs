module FsPolyJobs.Api.RestApi.RestApiPreamble

open Microsoft.AspNetCore.Mvc.ModelBinding
open Validus
open Microsoft.AspNetCore.Mvc

type RestApiError =
    | BadRequestOfErrors of ValidationErrors

let okEmpty (controller: ControllerBase) = controller.Ok() :> IActionResult
let ok (value: obj) (controller: ControllerBase) = controller.Ok(value) :> IActionResult

let badRequestEmpty (controller: ControllerBase) =
    controller.BadRequest() :> IActionResult

let badRequest (value: obj) (controller: ControllerBase) =
    controller.BadRequest(value) :> IActionResult

let badRequestErrors (modelState: ModelStateDictionary) (controller: ControllerBase) =
    controller.BadRequest(modelState) :> IActionResult

module ValidationErrors =
    let toModelState (modelState: ModelStateDictionary) (errors: ValidationErrors) =
        errors
        |> ValidationErrors.toMap
        |> Map.toSeq
        |> Seq.iter (fun ((key, errors)) ->
            errors
            |> Seq.iter (fun error -> modelState.AddModelError(key, error)))

    let badRequest (errors: ValidationErrors) (modelState: ModelStateDictionary) (controller: ControllerBase) =
        errors |> toModelState modelState
        badRequestErrors modelState controller

let toActionResult (result: Result<'a, RestApiError>) (controller: ControllerBase) =
    match result with
    | Error (BadRequestOfErrors errors) -> ValidationErrors.badRequest errors controller.ModelState controller
    | Ok value -> ok value controller
