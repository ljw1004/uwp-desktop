Imports System.IO

Public Class Form1
    Private Async Sub btnWinrt_Click(sender As Object, e As EventArgs) Handles btnWinrt.Click
        ' basics
        Dim folder = Windows.Storage.KnownFolders.DocumentsLibrary
        Dim opts As New Windows.Storage.Search.QueryOptions(
                              Windows.Storage.Search.CommonFileQuery.OrderByName, {".txt"})
        Dim files = Await folder.CreateFileQueryWithOptions(opts).GetFilesAsync(0, 20)
        For Each file In files
            textBox1.Text &= Path.GetFileName(file.Path) & vbCrLf
        Next

        ' streams
        Using reader = New IO.StreamReader(Await files.First.OpenStreamForReadAsync())
            Dim txt = Await reader.ReadToEndAsync()
            textBox1.Text &= txt.Substring(0, 100) & vbCrLf
        End Using

        ' pictures
        Dim pics = Await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync(
                         Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, 0, 1)
        Using stream = Await pics.First.OpenReadAsync()
            Dim decoder = Await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream)
            textBox1.Text &= decoder.OrientedPixelWidth & "x" & decoder.OrientedPixelHeight & vbCrLf
        End Using
    End Sub

    Private Sub btnAppx_Click(sender As Object, e As EventArgs) Handles btnAppx.Click

    End Sub
End Class
