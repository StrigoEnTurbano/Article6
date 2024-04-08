#r "nuget: Thoth.Json.Net"
#r "nuget: Hopac, 0.5.0"

let inline (^) f x = f x

let godotVersion = "4.2.1"

// Godot:
// - Engines: // Папка с различными версиями godot.
//   - Godot_v{godotVersion}-stable_mono_win64
//     - Godot_v{godotVersion}-stable_mono_win64.exe
// - Projects:
//  - ProjectName: // = godotProjectDirectory
//    - ProjectName.csproj
//    - ProjectName.sln
//    - ProjectName.Core: 
//      - ProjectName.Core.fsproj
//      - PrepareLaunchSettings.fsx
//    - Properties: // Изначально может отсутствовать.
//      - launchSettings.json

let godotProjectDirectory = System.IO.Path.GetDirectoryName __SOURCE_DIRECTORY__

[<RequireQualifiedAccess>]
type Run =
    | Editor
    | Game
    | Scene of Path : string

    with
    member this.CommandLineArgs = [
        "--path"
        "."

        let runOptions = [
            //"--debug-collisions"
            //"--debug-paths"
            //"--debug-navigation"
            //"--debug-avoidance"
            //"--debug-canvas-item-redraw"
            ]

        match this with
        | Run.Editor ->
            "--editor"
            //"--rendering-engine"
            //"opengl3"
        | Run.Game ->
            yield! runOptions
        | Run.Scene path ->
            path
            yield! runOptions
        "--verbose"
    ]
    member this.Name =
        match this with
        | Run.Editor -> "Godot Editor"
        | Run.Game -> "Godot Game"
        | Run.Scene path -> $"Godot {path}"


let pathToGodotExe =
    System.IO.Path.Combine(
        godotProjectDirectory // Godot/Projects/ProjectName
        , "../.." // Godot
        , $"Engines/Godot_v%s{godotVersion}-stable_mono_win64/Godot_v%s{godotVersion}-stable_mono_win64.exe"
    )
    |> System.IO.Path.GetFullPath

if not ^ System.IO.File.Exists pathToGodotExe then
    failwith $"%s{pathToGodotExe} not found."

open Thoth.Json.Net

[
    Run.Editor
    Run.Game

    let scenes =
        System.IO.Directory.EnumerateFiles(
            godotProjectDirectory
            , "*.tscn"
            , System.IO.SearchOption.AllDirectories
        )
    for fullPath in scenes do
        System.IO.Path.GetRelativePath(godotProjectDirectory, fullPath)
        |> Run.Scene
]
|> Seq.map ^ fun p ->
    p.Name
    , Encode.object [
        "commandName", Encode.string "Executable"
        "executablePath", Encode.string pathToGodotExe
        "commandLineArgs", Encode.string ^ String.concat " " p.CommandLineArgs
        "workingDirectory", Encode.string godotProjectDirectory
    ]
|> Seq.append [
    // Воспроизводит профиль по умолчанию, однако мне он ни разу не пригодился.
    System.IO.Directory.EnumerateFiles(godotProjectDirectory, "*.csproj")
    |> Seq.exactlyOne
    |> System.IO.Path.GetFileNameWithoutExtension
    , Encode.object [
        "commandName", Encode.string "Project"
    ]
]
|> fun profiles ->
    Encode.object [
        "profiles"
        , Encode.object ^ List.ofSeq profiles
    ]
|> Encode.toString 2
|> fun content ->
    let propertiesDir = 
        System.IO.Path.Combine(
            godotProjectDirectory
            , "Properties"
        )
    if not ^ System.IO.Directory.Exists propertiesDir then
        System.IO.Directory.CreateDirectory propertiesDir
        |> ignore
    System.IO.File.WriteAllText(
        System.IO.Path.Combine(
            propertiesDir
            , "launchSettings.json"
        )
        , content
    )