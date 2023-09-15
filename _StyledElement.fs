namespace MatAvFab

open System.Runtime.CompilerServices
open Avalonia
open Avalonia.Input.TextInput
open Avalonia.Styling
open Fabulous
open Fabulous.Avalonia

module StyledElement =

    let Theme =
        Attributes.defineSimpleScalarWithEquality<string> "StyledElement_ThemeKey" (fun _ newValueOpt node ->
            let target = node.Target :?> StyledElement

            match newValueOpt with
            | ValueNone -> target.Theme <- null
            | ValueSome themeKey ->
                let styles = Application.Current.Styles |> Seq.head :?> Styles

                match styles.TryGetResource(themeKey, ThemeVariant.Default) with
                | true, controlTheme -> target.Theme <- controlTheme :?> ControlTheme
                | _ -> ())

[<Extension>]
type StyledElementModifiers =

    [<Extension>]
    static member inline theme(this: WidgetBuilder<'msg, #IFabStyledElement>, value: string) =
        this.AddScalar(StyledElement.Theme.WithValue(value))
