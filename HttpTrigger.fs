namespace FunctionApp
 
 module FunctionModule =
    open Microsoft.AspNetCore.Mvc
    open Microsoft.Azure.WebJobs
    open Microsoft.AspNetCore.Http
    open System.IO
    open Microsoft.Extensions.Logging
    open TLEReader

    [<FunctionName("ReadTle")>]
    let Run ([<HttpTrigger(Methods=[|"POST"|])>] req:HttpRequest) (log:ILogger) = 
        async {
            "Runnning Function"
            |> log.LogInformation

            let! body = 
                new StreamReader(req.Body) 
                |> (fun stream -> stream.ReadToEndAsync()) 
                |> Async.AwaitTask

            let result =
                match body with
                | null -> BadRequestResult() :> ActionResult
                | "" -> BadRequestResult() :> ActionResult
                | _ -> JsonResult(TwoLineElementReader.parseSatelliteInformations body) :> ActionResult

            return result

        } |> Async.StartAsTask
