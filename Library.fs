namespace TLEReader


module FirstLineReader = 
    let getSatelliteNumber (line:string) = int line.[2..6]
    let getClassification (line:string) = line.[7]

module InternationalDesignatorReader =
    let getLaunchYear (line:string) = int line.[9..10]
    let getLaunchNumberInYear (line:string) = int line.[11..13]
    let getLaunchPiece (line:string) = int line.[14..16]

module TwoLineElementReader =
    type Classification = Unclassified = 'U' | Classified = 'C' | Secret = 'S'
    type InternationalDesignator = { LaunchYear:int;LaunchNumberInYear:int; LaunchPiece:string }
    type SatelliteInformations = {Name:string; Number:int;Classification:Classification;InternationalDesignator:InternationalDesignator}
    type TwoLineElement = {Satellite:SatelliteInformations}
    type Lines = TitleLine = 0 | Line1 = 1 | Line2 = 2

    let splitTleRawEntry(rawTle: string) = rawTle.Split ("\n",System.StringSplitOptions.RemoveEmptyEntries)

    let trimLine (tleLine: string)  = tleLine.Trim();
    
    let getLine (tleLines: string[], titleLine) = trimLine tleLines.[int titleLine]

    let toClassification value =
        match value with
            | 'U' -> Classification.Unclassified
            | 'S' -> Classification.Secret
            | 'C' -> Classification.Classified
            | _ -> Classification.Unclassified

    let satelliteInformations (rawTle) = 
       let lines = splitTleRawEntry rawTle
       let titleName = getLine (lines, Lines.TitleLine)
       let firstLine = getLine (lines, Lines.Line1)
       let secondLine = getLine (lines, Lines.Line2)

       //printfn "Title line %s FirstLine %s and second line %s" titleName firstLine secondLine

       {
        Name=titleName;
        Number= FirstLineReader.getSatelliteNumber firstLine;
        Classification= toClassification (FirstLineReader.getClassification firstLine);
        InternationalDesignator= { LaunchYear= InternationalDesignatorReader.getLaunchYear firstLine; LaunchPiece="A"; LaunchNumberInYear = 0 }
       }

    let ReadFromString value = printfn "You provided the following TLE value %s%s" System.Environment.NewLine value

module Main =
    let issTleValue = "ISS (ZARYA)\n
    1 25544U 98067A   08264.51782528 -.00002182  00000-0 -11606-4 0  2927\n
    2 25544  51.6416 247.4627 0006703 130.5360 325.0288 15.72125391563537"

    TwoLineElementReader.ReadFromString issTleValue
    printfn "%s" (TwoLineElementReader.satelliteInformations issTleValue).Name
    printfn "%d" (TwoLineElementReader.satelliteInformations issTleValue).Number
    printfn "%s" (string (TwoLineElementReader.satelliteInformations issTleValue).Classification)
