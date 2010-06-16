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

Namespace HostCommands.Runtime

    ''' <summary>
    ''' This class implements the NC command which
    ''' returns HSM diagnostics to the caller.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesSim.Core.HostCommands.ThalesCommandCode("NC", "ND", "", "Performs HSM diagnostics.")> _
    Public Class HSMDiagnostics_NC
        Inherits ThalesSim.Core.HostCommands.AHostCommand

        Public Overrides Function ConstructResponse() As ThalesSim.Core.Message.MessageResponse
            Dim mr As New ThalesSim.Core.Message.MessageResponse
            mr.AddElement(ThalesSim.Core.ErrorCodes._00_NO_ERROR)
            mr.AddElement(Convert.ToString(ThalesSim.Core.Resources.GetResource(ThalesSim.Core.Resources.LMK_CHECK_VALUE)))
            mr.AddElement(Convert.ToString(ThalesSim.Core.Resources.GetResource(ThalesSim.Core.Resources.FIRMWARE_NUMBER)))
            Return mr
        End Function

    End Class

End Namespace
