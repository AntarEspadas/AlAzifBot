module AlAzif.Core.Procedures.PlayerProcedures

open DSharpPlus.Entities
open Lavalink4NET.Players
open Lavalink4NET.Players.Queued
open Lavalink4NET.Tracks

let getTracks (player: IQueuedLavalinkPlayer) =
    let queue =
        player.Queue
        |> Seq.map (_.Track)
        |> Seq.filter ((<>) null)
        |> Seq.rev
    
    player.CurrentTrack
    |> Option.ofObj
    |> Option.toList
    |> Seq.append queue
    |> Seq.toList

let embedTracks (tracks: LavalinkTrack list) =
    let builder = DiscordEmbedBuilder()
    tracks
    |> Seq.rev
    |> Seq.iter (fun track ->
        builder.AddField(track.Title, track.Duration.ToString(), false) |> ignore
    )
    builder
    
let displayQueue (player: IQueuedLavalinkPlayer) =
    player
    |> getTracks
    |> embedTracks