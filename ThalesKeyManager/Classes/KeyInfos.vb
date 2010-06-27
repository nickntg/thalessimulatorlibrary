''' <summary>
''' Serves as a placeholder for all loaded keys.
''' </summary>
''' <remarks></remarks>
Public Class KeyInfos

    Protected Shared m_lst As New SortedList(Of String, KeyInfo)

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Return list with keys.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function InfoList() As SortedList(Of String, KeyInfo)
        Return m_lst
    End Function

    ''' <summary>
    ''' Read all keys from a file.
    ''' </summary>
    ''' <param name="fileName">Full file name.</param>
    ''' <remarks></remarks>
    Public Shared Sub ReadFromFile(ByVal fileName As String)
        m_lst.Clear()
        Using SR As IO.StreamReader = New IO.StreamReader(fileName, System.Text.Encoding.Default)
            While SR.Peek > -1
                Dim fileLine As String = SR.ReadLine
                Dim KI As New KeyInfo(fileLine)
                m_lst.Add(KI.KeyName, KI)
            End While
        End Using
    End Sub

    ''' <summary>
    ''' Write all keys to a file.
    ''' </summary>
    ''' <param name="fileName">Full file name.</param>
    ''' <remarks></remarks>
    Public Shared Sub WriteToFile(ByVal fileName As String)
        Using SW As IO.StreamWriter = New IO.StreamWriter(fileName, False, System.Text.Encoding.Default)
            For Each keyName As String In m_lst.Keys
                SW.WriteLine(m_lst(keyName).GetFileLine)
            Next
        End Using
    End Sub

End Class
