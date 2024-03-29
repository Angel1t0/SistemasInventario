﻿Imports Ical.Net
Imports Ical.Net.CalendarComponents
Imports Ical.Net.DataTypes
Imports Ical.Net.Serialization


Public Class DeterminarRecurrencia
    Public Function ParsearRRULE(mensaje As Mensaje) As List(Of Occurrence)
        Try
            Dim calendarEvent = New CalendarEvent With {
                .Start = New CalDateTime(mensaje.StartDateTime),
                .End = New CalDateTime(mensaje.EndDateTime)
            }

            If Not String.IsNullOrEmpty(mensaje.RRULE) Then
                Dim rrule As String = mensaje.RRULE.Substring(6)
                Dim recurrencePattern = New RecurrencePattern(rrule)
                calendarEvent.RecurrenceRules.Add(recurrencePattern)
            End If

            Dim occurrences = CalendarEvent.GetOccurrences(mensaje.StartDateTime, mensaje.EndDateTime)
            Dim listaOcurriencias As New List(Of Occurrence)
            For Each occurrence In occurrences
                ' Aquí tendrías que programar el envío del correo para cada fecha de ocurrencia
                Dim fechaDeEnvio = occurrence.Period.StartTime.AsSystemLocal
                listaOcurriencias.Add(occurrence)
            Next
            Return listaOcurriencias
        Catch ex As Exception
            Console.WriteLine("Error al parsear la recurrencia: " & ex.Message)
        End Try
    End Function
End Class
