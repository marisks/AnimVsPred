open Game

[<EntryPoint>]
let main argv = 
    use g = new AnimVsPredGame()
    g.Run()
    0 // return an integer exit code
