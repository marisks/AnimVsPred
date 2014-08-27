module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open AnimVsPred
open AnimVsPred.Domain
open Actors

type AnimVsPredGame() as x =
    inherit Game()

    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    let animalTex = lazy(Some(x.Content.Load "animal.png"))
    let predatorTex = lazy(Some(x.Content.Load "predator.png"))

    let mutable scaleX = 0.0
    let mutable scaleY = 0.0

    let vectToVect2 (v:Vector) =
        let x = v.X * scaleX
        let y = v.Y * scaleY
        Vector2((float32 x), (float32 y))

    let createAnimalActor tex (a: Vector) =
        (tex, Animal, vectToVect2 a, Vector2(32.f, 32.f))

    let createPredatorActor tex (p: Vector) =
        (tex, Predator, vectToVect2 p, Vector2(32.f, 32.f))

    let createWorldObjects (state: Simulation) =
        lazy(
            let animalActors = state.Animals |> List.map (createAnimalActor animalTex.Value)
            let predatorActors = state.Predators |> List.map (createPredatorActor predatorTex.Value)
            List.append animalActors predatorActors
            |> List.map createActor
        )

    let mutable state = Simulation.CreateInitialState()

    let mutable worldObjects = createWorldObjects state

    let drawActor (sb:SpriteBatch) actor =
        if actor.Texture.IsSome then 
            do sb.Draw(actor.Texture.Value, actor.Position, Color.White)
        ()

    override x.Initialize() =
        do x.TargetElapsedTime <- System.TimeSpan.FromSeconds(1.0/20.0)
        do spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        do scaleX <- float x.GraphicsDevice.Viewport.Width / Domain.maxWidth
        do scaleY <- float x.GraphicsDevice.Viewport.Height / Domain.maxHeight
        do base.Initialize()
        ()

    override x.LoadContent() =
        do worldObjects.Force() |> ignore
        ()

    override x.Update (gameTime) =
        state <- state.Step()
        let c = worldObjects.Value
        do worldObjects <- createWorldObjects state
        do worldObjects.Force() |> ignore
        ()

    override x.Draw (gameTime) =
        do x.GraphicsDevice.Clear Color.Beige
        let drawActor' = drawActor spriteBatch
        do spriteBatch.Begin()
        worldObjects.Value
        |> List.iter drawActor'
        do spriteBatch.End()
        ()


