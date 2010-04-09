''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 

Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' The purpose of this application is to allow the execution of the 
''' unit test cases from the Windows/Linux command line.
''' 
''' <b>Important:</b> In order to successfully execute, test case
''' methods must not accept any parameters.
''' </summary>
''' <remarks></remarks>
Module Module1

    Dim total As Integer = 0, failed As Integer = 0, displayDetails As Boolean = False
    Dim WithEvents thales As ThalesSim.Core.ThalesMain

    Sub Main()
        Console.Write("Display details [Y/N]?")
        displayDetails = (Console.ReadLine.ToUpper = "Y")

        Dim pbt As New ThalesSim.Tests.PINBlockTests
        RunTestMethods(pbt)
        pbt = Nothing

        Dim enct As New ThalesSim.Tests.EncryptionTests
        RunTestMethods(enct)
        enct = Nothing

        Dim hct As New ThalesSim.Tests.HostCommandTests
        Try
            Console.WriteLine("Initializing server for unit tests (assuming ThalesParameters.xml in local directory)...")
            thales = New ThalesSim.Core.ThalesMain
            thales.StartUpWithoutTCP("ThalesParameters.xml")
            RunTestMethods(hct)
            thales.ShutDown()
        Catch ex As Exception
            failed += 1
            Console.WriteLine("***WARNING: Error initializing server for host command tests!")
            Console.WriteLine(ex.ToString)
        End Try

        Console.WriteLine("Total unit tests: " + total.ToString)
        If failed > 0 Then
            Console.WriteLine("***WARNING: " + failed.ToString + " unit tests have failed!")
        Else
            Console.WriteLine("All unit tests passed.")
        End If
    End Sub

    Private Sub RunTestMethods(ByVal o As Object)
        Dim methods() As Reflection.MethodInfo = o.GetType.GetMethods
        For Each method As Reflection.MethodInfo In methods
            If method.GetCustomAttributes(GetType(TestMethodAttribute), False).GetLength(0) > 0 Then
                Try
                    total += 1
                    Console.WriteLine("Executing " + method.Name + "...")
                    method.Invoke(o, New Object() {})
                Catch ex As Exception
                    failed += 1
                    Console.WriteLine(method.Name + " failed")
                    Console.WriteLine(ex.ToString)
                End Try

            End If
        Next
    End Sub

    Private Sub thales_MajorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles thales.MajorLogEvent
        If displayDetails Then Console.WriteLine(s)
    End Sub

    Private Sub thales_MinorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles thales.MinorLogEvent
        If displayDetails Then Console.WriteLine(s)
    End Sub

End Module
