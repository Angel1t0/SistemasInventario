﻿Imports Ical.Net.CalendarComponents
Imports Ical.Net.DataTypes
Imports System.Timers

Public Class GestionRecurrenciaMensajes
    Private WithEvents _timer As Timer
    Private _eventoControlador As EventoControlador
    Private _calendarID As String
    Private _determinarRecurrencia As DeterminarRecurrencia

    Public Sub New(calendarID As String)
        _timer = New Timer With {
            .Interval = 60000, ' Revisar cada minuto, ajusta según necesites
            .AutoReset = True
            }

        AddHandler _timer.Elapsed, AddressOf OnTimedEvent

        _eventoControlador = New EventoControlador()
        _calendarID = calendarID
        _determinarRecurrencia = New DeterminarRecurrencia()
    End Sub

    Public Sub Iniciar()
        _timer.Start()
    End Sub
    Public Sub Detener()
        _timer.Stop()
    End Sub
    Private Sub OnTimedEvent(sender As Object, e As ElapsedEventArgs)
        ' Aquí es donde llamarías a la función que verifica si es hora de enviar el correo.
        VerificarYEnviarCorreos()
    End Sub

    Private Sub VerificarYEnviarCorreos()
        ' 1. Obtener la lista de eventos y mensajes
        ' 2. Para cada evento, verificar si corresponde enviar algún mensaje basado en su recurrencia
        ' 3. Si es así, enviar el mensaje
        Try
            Dim eventosYMensajesData As List(Of EventoYMensajes) = _eventoControlador.ObtenerEventosYMensajes(_calendarID)
            If eventosYMensajesData.Count = 0 Then
                Return
            End If
            Dim horaActual As DateTime = DateTime.Now
            For Each eventoYMensajes In eventosYMensajesData
                Dim listaOcurriencias As List(Of Occurrence) = _determinarRecurrencia.ParsearRRULE(eventoYMensajes.Mensaje)
                If eventoYMensajes.Mensaje.SentTime.Date = Nothing Then
                    eventoYMensajes.Mensaje.SentTime = eventoYMensajes.Mensaje.StartDateTime
                End If
                For Each ocurrencia In listaOcurriencias
                    If ocurrencia.Period.StartTime.AsSystemLocal <= horaActual.Date And
                        eventoYMensajes.Mensaje.SentTime.Date < ocurrencia.Period.StartTime.AsSystemLocal.Date Then

                        eventoYMensajes.Mensaje.SentTime = ocurrencia.Period.StartTime.AsSystemLocal
                        ' Aquí es donde enviarías el correo
                        _eventoControlador.EnviarEmail(eventoYMensajes.Mensaje)
                        ' Tiene un limite de mensajes
                        '_eventoControlador.EnviarWhatsApp(eventoYMensajes.Evento.CreatorPhone, eventoYMensajes.Mensaje)
                        _eventoControlador.EnviarNotificacionDesktop(eventoYMensajes.Mensaje)

                        _eventoControlador.ActualizarFechaEnvio(eventoYMensajes.Mensaje.MessageID, eventoYMensajes.Mensaje.SentTime)
                    End If
                Next
            Next
        Catch ex As Exception
            Console.WriteLine("Error al verificar y enviar correos: " & ex.Message)
        End Try
    End Sub

End Class
