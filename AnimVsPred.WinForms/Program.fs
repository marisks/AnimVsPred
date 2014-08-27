open System
open System.Drawing
open System.Windows.Forms
open AnimVsPred.Domain

let form = new Form(Text = "Animals vs Predators", Width = 1000, Height = 800, Location = new Point(0, 0))
let timeLabel = new Label()
let drawTimeLabel = new Label(Location = new Point(100, 0))
form.Controls.Add(timeLabel)
form.Controls.Add(drawTimeLabel)

let g = form.CreateGraphics()

let drawState(state:Simulation, time) = 
    timeLabel.Text <- sprintf "Total time: %A" time

    let sw = System.Diagnostics.Stopwatch.StartNew()

    g.Clear(SystemColors.Control)
    let drawPredator x y =
        let rect = new Rectangle(x, y, 20, 20)
        g.DrawEllipse(Pens.Red, rect)

    let drawAnimal x y =
        let rect = new Rectangle(x, y, 10, 10)
        g.DrawEllipse(Pens.Green, rect)
        
    state.Animals
    |> List.map (fun a -> (drawAnimal (int a.X) (int a.Y)))
    |> ignore

    state.Predators
    |> List.map (fun a -> (drawPredator (int a.X) (int a.Y)))
    |> ignore

    sw.Stop()
    
    drawTimeLabel.Text <- sprintf "Draw time: %A" sw.Elapsed

    not form.IsDisposed

let rec runSimulation(state:Simulation, time) =
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let running = form.Invoke(new Func<bool>(fun () -> 
        drawState(state, time))) :?> bool
    if(running) then
        let state = state.Step()
        sw.Stop()
        let time = sw.Elapsed
        runSimulation(state, time)

Async.Start(async {
    runSimulation(Simulation.CreateInitialState(), TimeSpan.Zero)
})

Application.Run(form)
