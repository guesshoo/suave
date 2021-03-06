﻿module Suave.Tests.HttpFile

open Fuchu

open System
open System.IO
open System.Text

open Suave.Types
open Suave.Http
open Suave.Http.Successful

open Suave.Tests.TestUtilities
open Suave.Testing

[<Tests>]
let ``canonicalization attacks`` =
  testList "canonicalization attacks" [
    testCase "should throw" <| fun _ ->
      Assert.Raise("'../../passwd' is not a valid path",
        typeof<Exception>,
        fun _ -> Files.resolvePath currentPath "../../passwd" |> ignore)
  ]

[<Tests>]
let compression =
  let runWithConfig = runWith defaultConfig

  let testFileSize = (new FileInfo(Path.Combine(currentPath,"test-text-file.txt"))).Length

  testList "getting basic gzip/deflate responses" [
      testCase "200 OK returns 'Havana' with gzip " <| fun _ ->
        Assert.Equal("expecting 'Havana'", "Havana", runWithConfig (OK "Havana") |> reqGZip HttpMethod.GET "/" None)

      testCase "200 OK returns 'Havana' with deflate " <| fun _ ->
        Assert.Equal("expecting 'Havana'", "Havana", runWithConfig (OK "Havana") |> reqDeflate HttpMethod.GET "/" None)

      testCase "verifiying we get the same size uncompressed" <| fun _ ->
        Assert.Equal("length should match"
        , testFileSize
        , (runWithConfig (Files.browseFileHome "test-text-file.txt") |> reqBytes HttpMethod.GET "/" None).Length |> int64)

      testCase "gzip static file" <| fun _ ->
        Assert.Equal("length should match"
        , testFileSize
        , (runWithConfig (Files.browseFileHome "test-text-file.txt") |> reqGZipBytes HttpMethod.GET "/" None).Length |> int64)
    ]
