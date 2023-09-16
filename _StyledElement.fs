namespace MatAvFab

open System.Runtime.CompilerServices
open Avalonia
open Avalonia.Input.TextInput
open Avalonia.Styling
open Fabulous
open Fabulous.Avalonia

module StyledElement =

    let ThemeKey =
        Attributes.defineSimpleScalarWithEquality<string> "StyledElement_ThemeKey" (fun _ newValueOpt node ->
            let target = node.Target :?> StyledElement

            match newValueOpt with
            | ValueNone -> target.Theme <- null
            | ValueSome themeKey ->
                match Application.Current.Styles.TryGetResource(themeKey, null) with
                | true, value ->
                    match value with
                    | :? ControlTheme as controlTheme -> target.Theme <- controlTheme
                    | _ -> ()
                | _ -> ())

[<Extension>]
type StyledElementModifiers =

    [<Extension>]
    static member inline themeKey(this: WidgetBuilder<'msg, #IFabStyledElement>, value: string) =
        this.AddScalar(StyledElement.ThemeKey.WithValue(value))
