namespace FunctionApp
 
 module FunctionModule =
  open Microsoft.AspNetCore.Mvc
    open Microsoft.Azure.WebJobs
    open Microsoft.AspNetCore.Http
    open Newtonsoft.Json
    open System.IO
    open Microsoft.Extensions.Logging
    open TLEReader
    

    type User = {
        Tle: string
    }

    [<FunctionName("ReadTle")>]
    let Run ([<HttpTrigger(Methods=[|"POST"|])>] req:HttpRequest) (log:ILogger) = 
        async {
            "Runnning Function"
            |> log.LogInformation

            let! body = 
                new StreamReader(req.Body) 
                |> (fun stream -> stream.ReadToEndAsync()) 
                |> Async.AwaitTask

            let tle = JsonConvert.DeserializeObject<User>(body)
            return JsonResult(TwoLineElementReader.satelliteInformations tle.Tle)
        } |> Async.StartAsTask
