module FsPolyJobs.Api.RestApi.RestApiPreamble

open Microsoft.AspNetCore.Mvc.ModelBinding
open Validus
open Microsoft.AspNetCore.Mvc

type RestApiError =
    | BadRequestOfErrors of ValidationErrors
    | NotFoundEntity of id: string * entityName: string

let okEmpty (controller: ControllerBase) = controller.Ok() :> IActionResult
let ok (value: obj) (controller: ControllerBase) = controller.Ok(value) :> IActionResult

let badRequestEmpty (controller: ControllerBase) =
    controller.BadRequest() :> IActionResult

let badRequest (value: obj) (controller: ControllerBase) =
    controller.BadRequest(value) :> IActionResult

let badRequestErrors (modelState: ModelStateDictionary) (controller: ControllerBase) =
    controller.BadRequest(modelState) :> IActionResult

let notFoundEmpty (controller: ControllerBase) = controller.NotFound() :> IActionResult

let notFound (value: obj) (controller: ControllerBase) =
    controller.NotFound(value) :> IActionResult

module ProblemDetails =
    let fromNotFound (id: string) (entityName: string) =
        let result = ProblemDetails()
        result.Title <- sprintf "The %s with id %s was not found" entityName id
        result.Status <- 404
        result.Extensions.Add("id", id)
        result.Extensions.Add("entityName", entityName)
        result

    let notFound (id: string) (entityName: string) (controller: ControllerBase) =
        notFound (fromNotFound id entityName) controller

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
    | Error (NotFoundEntity (id, entityName)) -> ProblemDetails.notFound id entityName controller
    | Ok value -> ok value controller
