Was aufgefallen ist in Verbindung mit dem Ribbon mit WinForms:

ApplicationModes in Verbindung mit Button, SplitButton, DropDownButton sind nur im ApplicationMenu (Left Side) zul�ssig.
Das wird im RibbonDesigner nicht ber�cksichtigt.
Im Original RibbonDesigner sind ein paar Bugs, die in den RibbonTools korrigiert wurden.

Wenn ApplicationModes auf Groups oder Tabs verwendet werden, dann m�ssen alle! Tabs und Groups eine
Definition von ApplicationModes enthalten.
Ansonsten wird bei �ndern des Ribbon.SetModes ein Beenden der App durchgef�hrt.
Hier vermute ich einen Bug in der Microsoft Com Implementierung.
Die UICC.exe stellt keinen Fehler fest.

Im RibbonDesigner w�re es einfacher, wenn in der View auch ein Command vergeben werden k�nnte
mit allen notwendigen Eingaben f�r Texte und Images. Oder auch ein Aufruf bei vorhandenem Command f�r die Eingabe von Text, Images
=> einfacher Wechsel zum Command m�glich mit R�ckkehr an die verlassene View

Copy Funktion f�r SizeDefinition (Large  nach Medium oder Small)

Microsoft Bug in UICC.exe mit SizeDefinition ButtonGroupsAndInputs, Beispiel aus Dokumentation funktioniert nicht.
Entfernen der ControlGroup mit Button10 akzeptiert UICC, aber der Button3 erscheint dann an falscher Stelle