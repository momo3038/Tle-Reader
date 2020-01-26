namespace TLEReader

module FirstLineReader =
    let getSatelliteNumber (line: string) = int line.[2..6]
    let getClassification (line: string) = line.[7]
    let getRadiationPressureCoef (line: string) = line.[53..60]
    let getEphemerisType (line: string) = line.[62]
    let getElementSetNumber (line: string) = int line.[64..67]

module InternationalDesignatorReader =
    let getLaunchYear (line: string) = int line.[9..10]
    let getLaunchNumberInYear (line: string) = int line.[11..13]
    let getLaunchPiece (line: string) = string line.[14..16]

module EpochReader =
    let getYear (line: string) = int line.[18..19]
    let getDay (line: string) = float line.[20..31]

module MeanMotionReader =
    let getFirstDerivative (line: string) = float line.[33..42]
    let getSecondDerivative (line: string) = string line.[44..52]

module TwoLineElementReader =
    type Classification =
        | Unclassified = 'U'
        | Classified = 'C'
        | Secret = 'S'

    type InternationalDesignator =
        { LaunchYear: int
          LaunchNumberInYear: int
          LaunchPiece: string }

    type Epoch =
        { Year: int
          Day: float }

    type MeanMotion =
        { FirstDerivative: float
          SecondDerivative: string }

    type SatelliteInformations =
        { Name: string
          Number: int
          Classification: Classification
          InternationalDesignator: InternationalDesignator
          Epoch: Epoch
          MeanMotion: MeanMotion
          RadiationPressureCoef: string
          EphemerisType: char
          ElementSetNumber: int }

    type TwoLineElement =
        { Satellite: SatelliteInformations }

    type Lines =
        | TitleLine = 0
        | Line1 = 1
        | Line2 = 2

    let splitTleRawEntry (rawTle: string) = rawTle.Split("\n", System.StringSplitOptions.RemoveEmptyEntries)

    let trimLine (tleLine: string) = tleLine.Trim()

    let getLine (tleLines: string [], titleLine) = trimLine tleLines.[int titleLine]

    let toClassification value =
        match value with
        | 'U' -> Classification.Unclassified
        | 'S' -> Classification.Secret
        | 'C' -> Classification.Classified
        | _ -> Classification.Unclassified

    let parseSatelliteInformations rawTle =
        let lines = splitTleRawEntry rawTle
        let titleName = getLine (lines, Lines.TitleLine)
        let firstLine = getLine (lines, Lines.Line1)
        let secondLine = getLine (lines, Lines.Line2)

        { Name = titleName
          Number = FirstLineReader.getSatelliteNumber firstLine
          Classification = toClassification (FirstLineReader.getClassification firstLine)
          InternationalDesignator =
              { LaunchYear = InternationalDesignatorReader.getLaunchYear firstLine
                LaunchPiece = InternationalDesignatorReader.getLaunchPiece firstLine
                LaunchNumberInYear = InternationalDesignatorReader.getLaunchNumberInYear firstLine }
          Epoch =
              { Year = EpochReader.getYear firstLine
                Day = EpochReader.getDay firstLine }
          MeanMotion =
              { FirstDerivative = MeanMotionReader.getFirstDerivative firstLine
                SecondDerivative = MeanMotionReader.getSecondDerivative firstLine }
          RadiationPressureCoef = FirstLineReader.getRadiationPressureCoef firstLine
          EphemerisType = FirstLineReader.getEphemerisType firstLine
          ElementSetNumber = FirstLineReader.getElementSetNumber firstLine
        }

module Display =
    open TwoLineElementReader

    let DisplayTleToConsole rawLine =
        printfn "You provided the following TLE value %s%s" System.Environment.NewLine rawLine

    let DisplayToConsole satInfos =
        printfn "Satellite name is %s with a unique number %d and his classification is %s" satInfos.Name
            satInfos.Number (string (satInfos.Classification))

        printfn "Launch year is %d and the piece of the launch is %s. It was the %d launch of the year"
            satInfos.InternationalDesignator.LaunchYear
            satInfos.InternationalDesignator.LaunchPiece
            satInfos.InternationalDesignator.LaunchNumberInYear

        printfn "Epoch Year %d | Day %f" satInfos.Epoch.Year satInfos.Epoch.Day

        printfn "Mean motion First derivative %f Second derivative %s" satInfos.MeanMotion.FirstDerivative
            satInfos.MeanMotion.SecondDerivative

        printf "Radiation pressure coef is %s, Ephemeris type is %c and Element set number is %d" satInfos.RadiationPressureCoef 
            satInfos.EphemerisType satInfos.ElementSetNumber

module Main =
    open Display
    open TwoLineElementReader

    let issTleValue = "ISS (ZARYA)\n
    1 25544U 98067A   08264.51782528 -.00002182  00000-0 -11606-4 0  2927\n
    2 25544  51.6416 247.4627 0006703 130.5360 325.0288 15.72125391563537"

    issTleValue |> DisplayTleToConsole
    parseSatelliteInformations issTleValue |> DisplayToConsole
