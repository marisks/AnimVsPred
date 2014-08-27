namespace AnimVsPred
open System.Threading.Tasks
open FSharp.Collections.ParallelSeq

module Domain =
    
    [<Struct>]
    type Vector(x:float, y:float) =
        member t.X = x
        member t.Y = y
        static member (+) (vect1:Vector, vect2:Vector) =
            Vector(vect1.X + vect2.X, vect1.Y + vect2.Y)
        static member (*) (vect:Vector, f) =
            Vector(vect.X*f, vect.Y*f)
        static member (-) (vect1:Vector, vect2:Vector) =
            vect1 + (vect2 * -1.0)

    type Simulation = {
        Animals : list<Vector>
        Predators: list<Vector>
    }

    let maxWidth = 2000.0
    let maxHeight = 2000.0

    let randomLocations (count:int) =
        let rnd = SafeRandom.New()
        Seq.init count (fun _ -> Vector(rnd.NextDouble()*maxWidth, rnd.NextDouble()*maxHeight))


    let distance (vect1:Vector) (vect2:Vector) =
        let x = abs(vect1.X - vect2.X)
        let y = abs(vect1.Y - vect2.Y)
        sqrt(x**2.0 + y**2.0)

    let getPathPoints(count:int, vect1:Vector, vect2:Vector) =
        let dist = distance vect1 vect2
        let step = dist / float(count)
        let vsum = vect1 + vect2;
        Seq.init count (fun i -> 
            let x = vsum.X * float(i) / float(count)
            let y = vsum.Y * float(i) / float(count)
            Vector(x, y))


    let moveAnimal (state:Simulation) (animPos:Vector) =
        let nearestPredatorDistanceFrom(pos) =
            state.Predators
            |> Seq.map (distance pos) |> Seq.min

        let nearestPredatorDistanceOnPath(target) =
            getPathPoints(10, animPos, target)
            |> Seq.map nearestPredatorDistanceFrom |> Seq.min

        let target = 
            randomLocations(10)
            |> Seq.maxBy nearestPredatorDistanceOnPath

        animPos + (target - animPos) * (20.0 / (distance target animPos))

    let movePredator (state:Simulation) (predPos:Vector) =
        let countCloseLocations (an:seq<Vector>) (pos:Vector) =
            an
            |> Seq.filter (fun x -> (distance x pos) < 50.0)
            |> Seq.length

        let countCloseLocationsOnPath(an, pfrom, ptarget) =
            getPathPoints(10, pfrom, ptarget)
            |> Seq.sumBy (countCloseLocations an)

        let target = 
            randomLocations(20)
            |> Seq.maxBy (fun pos -> 
                countCloseLocationsOnPath(state.Animals, predPos, pos) -
                countCloseLocationsOnPath(state.Predators, predPos, pos) * 3)

        predPos + (target - predPos) * (10.0 / (distance target predPos))

    let simulationStep(state) =
        let animals = Task.Factory.StartNew(fun () ->
            state.Animals
                |> PSeq.map (moveAnimal state)
                |> List.ofSeq)
        let predators =
            state.Predators
                |> PSeq.map (movePredator state)
                |> List.ofSeq
        { Animals = animals.Result; Predators = predators }

    type Simulation with
        static member CreateInitialState() =
            { 
                Animals = randomLocations(70) |> Seq.toList
                Predators = randomLocations(7) |> Seq.toList
            }

        member x.Step() =
            simulationStep(x)