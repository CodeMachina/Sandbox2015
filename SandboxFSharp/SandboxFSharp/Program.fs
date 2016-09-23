// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 
    //print arguments passed to the executing program
//    printfn "%A" argv
//
//    //take in input and print it
//    let input = System.Console.ReadLine()
//    printfn "%s" input
//
//    //Create function
//    let square x = x * x
//    let printResponse message value = printfn "%s :: %i" message value
//    let message = "Blah some stuff"
//    printResponse message 4
//
//    //pipe value or function result to next function.  lambda format (fun param -> do stuff)
//    4 |> (fun x -> x * x)  |> printResponse message 
    

    let square x = x * x

    let sumOfSquares n = 
        [1..n] |> List.map square |> List.sum

    let result = sumOfSquares 100

    printfn "%i" result

    //ignore ... ignores the result of the function
    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
