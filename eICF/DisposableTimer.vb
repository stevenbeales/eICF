
Namespace ePS.eICF
    Public Class DisposableTimer
        Implements IDisposable

        ''' <summary>
        ''' Timer class that disposes of its resources like a managed resource
        ''' </summary>
        ''' <param name="callback"></param>
        Public Sub New(callback As Threading.TimerCallback)
            _Timer = New Threading.Timer(callback)
        End Sub

        Public Function Change(dueTime As Integer, period As Integer) As Boolean
            Return _Timer.Change(dueTime, period)
        End Function

        Protected Overridable Overloads Sub Dispose(
      ByVal disposing As Boolean)
            If Not Me.disposed Then
                ' Add code here to release the unmanaged resource.
                _Timer.Dispose()
                ' Note that this is not thread safe. 
            End If
            Me.disposed = True
        End Sub

        ' Do not change or add Overridable to these methods. 
        ' Put cleanup code in Dispose(ByVal disposing As Boolean). 
        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub

        Protected disposed As Boolean = False
        Private _Timer As Threading.Timer
    End Class
End Namespace

