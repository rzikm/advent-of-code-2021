module Array

open FSharpPlus

let mapAt i f array =
    Array.updateAt i (Array.item i array |> f) array

let item2d x y array = array |> Array.item y |> Array.item x

let item2dp (x, y) array = item2d x y array

let tryItem2d x y array =
    array |> Array.tryItem y |> Option.bind (Array.tryItem x)

let tryItem2dp (x, y) array = tryItem2d x y array

let neighbors2d8 x y array =
    Tuple2.neighbors8 (x, y) |> Seq.choose (fun (x, y) -> tryItem2d x y array)

let neighbors2d4p p array =
    Tuple2.neighbors4 p |> Seq.choose (flip tryItem2dp array)

let item3d x y z array =
    array |> Array.item z |> Array.item y |> Array.item x

let tryItem3d x y z array =
    array |> Array.tryItem z |> Option.bind (Array.tryItem y) |> Option.bind (Array.tryItem x)

let neighbors3d26 x y z array =
    Tuple3.neighbors26 (x, y, z) |> Seq.choose (fun (x, y, z) -> tryItem3d x y z array)

let item4d x y z w array =
    array |> Array.item w |> Array.item z |> Array.item y |> Array.item x

let mapAt2dp (x, y) f array = array |> mapAt y (mapAt x f)

let allIndexes array = seq { 0 .. Array.length array - 1 }

let allIndexes2d array =
    seq {
        for y in allIndexes array do
            for x in Array.item y array |> allIndexes do
                yield x, y
    }

let bounds2d array =
    Array.length (Array.item 0 array), Array.length array

let allIndexesInLine len array =
    let (xx, yy) = bounds2d array

    seq {
        for x0 in 0 .. xx - 1 do
            for y0 in 0 .. yy - 1 do
                if x0 + len <= xx then
                    yield [ for d in 0 .. len - 1 -> (x0 + d, y0) ]

                if y0 + len <= yy then
                    yield [ for k in 0 .. len - 1 -> (x0, y0 + k) ]

                if x0 + len <= xx && y0 + len <= yy then
                    yield [ for k in 0 .. len - 1 -> (x0 + k, y0 + k) ]
                    yield [ for k in 0 .. len - 1 -> (x0 + len - 1 - k, y0 + k) ]

    }

let isInBounds2d (x, y) array =
    y >= 0 && y < Array.length array && x >= 0 && x < (Array.item y array |> Array.length)

let tryFindIndex2d f array =
    let lenx, leny = bounds2d array

    let rec loop x y =
        if x >= lenx then loop 0 (y + 1)
        elif y >= leny then None
        elif f (item2d x y array) then Some(x, y)
        else loop (x + 1) y

    loop 0 0

let findIndex2d f array = tryFindIndex2d f array |> Option.get

let tryItem4d x y z w array =
    array
    |> Array.tryItem w
    |> Option.bind (Array.tryItem z)
    |> Option.bind (Array.tryItem y)
    |> Option.bind (Array.tryItem x)

let neighbors4d x y z w array =
    seq {
        for ww in w - 1 .. w + 1 do
            for zz in z - 1 .. z + 1 do
                for yy in y - 1 .. y + 1 do
                    for xx in x - 1 .. x + 1 do
                        if xx <> x || yy <> y || zz <> z || ww <> w then
                            yield (xx, yy, zz, ww)
    }
    |> Seq.choose (fun (x, y, z, w) -> tryItem4d x y z w array)

let init2d lenX lenY f =
    Array.init lenY (fun y -> Array.init lenX (fun x -> f x y))

let map2d f array =
    let leny = Array.length array
    let lenx = Array.item 0 array |> Array.length

    init2d lenx leny (fun x y -> f x y (item2d x y array))

let rotate2dClockwise array =
    let leny = Array.length array
    let lenx = Array.item 0 array |> Array.length

    init2d leny lenx (fun x y -> item2d y (leny - 1 - x) array)

let rotate2dCounterclockwise array =
    let leny = Array.length array
    let lenx = Array.item 0 array |> Array.length

    init2d leny lenx (fun x y -> item2d (lenx - 1 - y) x array)

module Tests =
    open Xunit
    open FsUnit.Xunit

    [<Fact>]
    let ``Array.rotate2dClockwise rotates correctly`` () =
        let original = [| [| 1; 2; 3 |]; [| 4; 5; 6 |] |]

        let expected = [| [| 4; 1 |]; [| 5; 2 |]; [| 6; 3 |] |]

        rotate2dClockwise original |> should equal expected

    [<Fact>]
    let ``Array.rotate2dCounterclockwise rotates correctly`` () =
        let original = [| [| 1; 2; 3 |]; [| 4; 5; 6 |] |]

        let expected = [| [| 3; 6 |]; [| 2; 5 |]; [| 1; 4 |] |]

        rotate2dCounterclockwise original |> should equal expected
