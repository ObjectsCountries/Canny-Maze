﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wawa.Modules;
using Wawa.Extensions;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;

public class UncannyMazeTP : Twitch<UncannyMaze>
{
    void Start()
    {
        Module.GetComponent<KMBombModule>().Add(onActivate: () =>
        {
            if (IsTP)
                Module.animSpeed = 2;
        }, onPass: () =>
        {
            if (!IsTP && Module.music)
                Module.Play(new Sound("uncanny" + UncannyMazeTile.current.uncannyValue));
        }
        );
    }

    [Command("")]
    IEnumerable<Instruction> Press(string movement)
    {
        List<KMSelectable> buttons = new List<KMSelectable>();
        if (!Module.mazeGenerated && !Module.tookTooLong)
        {
            yield return TwitchString.SendToChatError("{0}, please wait for the maze to generate.");
            yield break;
        }
        yield return null;
        movement = movement.ToLowerInvariant();
        switch (movement)
        {
            case "m":
            case "maze":
                yield return new KMSelectable[] { Module.maze };
                yield break;
            case "n":
            case "numbers":
                if (!Module.viewingWholeMaze)
                    buttons.Add(Module.maze);
                buttons.Add(Module.numbersButton);
                yield return buttons.ToArray();
                yield break;
            case "reset":
                if (Module.viewingWholeMaze)
                    buttons.Add(Module.maze);
                buttons.Add(Module.resetButton);
                yield return buttons.ToArray();
                yield break;
            default:
                break;
        }
        buttons.Clear();
        if (Module.viewingWholeMaze)
            buttons.Add(Module.maze);
        foreach (char c in movement)
        {
            switch (c)
            {
                case 'u':
                    buttons.Add(Module.arrowup);
                    break;
                case 'd':
                    buttons.Add(Module.arrowdown);
                    break;
                case 'l':
                    buttons.Add(Module.arrowleft);
                    break;
                case 'r':
                    buttons.Add(Module.arrowright);
                    break;
                case 'a':
                    buttons.Add(Module.appendButton);
                    break;
                case ' ':
                    break;
                default:
                    yield return TwitchString.SendToChatError("{0}, " + c.ToString() + " is an invalid character.");
                    yield break;
            }
        }
        yield return buttons.ToArray();
    }

    public override IEnumerable<Instruction> ForceSolve()
    {
        if (!Module.mazeGenerated && !Module.tookTooLong)
            yield break;
        Module.Log("Force solved by Twitch mod.");
        if (Module.tookTooLong)
        {
            Module.numbersButton.OnInteract();
            yield return new WaitForSeconds(.05f);
            yield break;
        }
        if (Module.viewingWholeMaze)
        {
            Module.maze.OnInteract();
            yield return new WaitForSeconds(.05f);
        }
        Module.resetButton.OnInteract();
        yield return new WaitForSeconds(.05f);
        foreach (string dir in Module.correctPath)
        {
            switch (dir)
            {
                case "up":
                    Module.arrowup.OnInteract();
                    yield return new WaitForSeconds(.05f);
                    break;
                case "down":
                    Module.arrowdown.OnInteract();
                    yield return new WaitForSeconds(.05f);
                    break;
                case "left":
                    Module.arrowleft.OnInteract();
                    yield return new WaitForSeconds(.05f);
                    break;
                case "right":
                    Module.arrowright.OnInteract();
                    yield return new WaitForSeconds(.05f);
                    break;
                case "append":
                    Module.appendButton.OnInteract();
                    yield return new WaitForSeconds(.05f);
                    break;
            }
        }
    }
}
