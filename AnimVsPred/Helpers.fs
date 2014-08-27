namespace AnimVsPred

module SafeRandom =
    open System

    let private rnd =
        new Random()

    let New() =
        lock rnd (fun () ->
            new Random(rnd.Next())
        )
