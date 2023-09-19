namespace MatAvFab

open System
open Fabulous
open Fabulous.Avalonia
open Material.Avalonia

open type Fabulous.Avalonia.View
open Material.Colors
open Material.Styles.Themes

module App =
    type Model =
        { Count: int; Step: int; TimerOn: bool }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | SetStep of float
        | TimerToggled of bool
        | TimedTick
        | TextChanged of string
    let initModel = { Count = 0; Step = 1; TimerOn = false }

    let timerCmd () =
        async {
            do! Async.Sleep 200
            return TimedTick
        }
        |> Cmd.ofAsyncMsg

    let init () =
        FabApplication.Current.Styles.Clear()
        FabApplication.Current.Styles.Add(MaterialTheme(null, PrimaryColor = PrimaryColor.LightBlue, SecondaryColor = SecondaryColor.Amber))
        initModel, Cmd.none

    let update msg model =
        match msg with
        | Increment ->
            { model with
                Count = model.Count + model.Step },
            Cmd.none
        | Decrement ->
            { model with
                Count = model.Count - model.Step },
            Cmd.none
        | Reset -> initModel, Cmd.none
        | SetStep n -> { model with Step = int(n + 0.5) }, Cmd.none
        | TimerToggled on -> { model with TimerOn = on }, (if on then timerCmd() else Cmd.none)
        | TimedTick ->
            if model.TimerOn then
                { model with
                    Count = model.Count + model.Step },
                timerCmd()
            else
                model, Cmd.none
        | TextChanged _ ->
            model, Cmd.none

    let view model =
        
        (VStack(10.) {
            TextBlock($"%d{model.Count}").centerText()

            Button("Increment", Increment).centerHorizontal()

            Button("Decrement", Decrement).centerHorizontal()

            (HStack() {
                TextBlock("Timer").centerVertical()

                ToggleSwitch(model.TimerOn, TimerToggled)
            })
                .margin(20.)
                .centerHorizontal()

            Slider(1., 10, float model.Step, SetStep)

            TextBlock($"Step size: %d{model.Step}").centerText()

            Button("Reset", Reset)
                .classes("accent")
                .centerHorizontal()
            |> (fun t -> if model.Count % 2 = 1 then t.themeKey("ButtonHoveredOpacity") else t.themeKey("MaterialOutlineButton"))

            
            TextBox("", TextChanged)
                .fontSize(10.)
                .padding(0.)
            |> (fun t -> if model.Count % 2 = 1 then t.themeKey("FilledTextBox") else t)

        })
            .center()

    
#if MOBILE
    let app model = SingleViewApplication(view model)
#else
    let app model = DesktopApplication(Window(view model))
#endif
    
    let program =
        Program.statefulWithCmd init update app
        |> Program.withLogger
                { ViewHelpers.defaultLogger() with
                    MinLogLevel = LogLevel.Debug }
        |> Program.withTrace (fun (format, msg) -> printfn $" ****************%A{format} %A{box msg}")
