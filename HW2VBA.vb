Option Explicit
Sub Stock()

Dim wb As Workbook
Dim sh As Worksheet
Dim MyStocks As Dictionary
Dim Stock As clsStock
Dim strTicker As String
Dim dtDay As Date
Dim lgChange As Double
Dim varKey As Variant

Dim lastrow As Long
Dim i As Long
Dim j As Long
Set wb = ThisWorkbook
' For each worksheet in this workbook do the following.
For Each sh In wb.Worksheets
    'Wanted to use list ojects but could not EXCEL was out of memory.
    'So used manual iteration through the items on the sheet.
    'Create Dictonary to hold summary of stock activity.
    Set MyStocks = New Dictionary
    sh.Select
    'Using the worksheet fuction, as the example from class was tempermental using the "xlup"
    lastrow = Application.WorksheetFunction.CountA(Range("A:A"))
    i = 2
    'Start on row after header and go through all rows
    Do Until i = lastrow
        strTicker = Cells(i, 1).Value
        'Convert provided date to date datatype.
        dtDay = Right(Cells(i, 2).Value, 2) & "/" & Mid(Cells(i, 2).Value, 5, 2) & "/" & Left(Cells(i, 2).Value, 4)
            'Check to see if ticker exists in dictionary,
            'If it exists then check if the date is earlier or later than the data previously reviewd
            'Homework requests the summary of the year's change not the max and min.
            'So reviewing for the highest earliest open date and latest closed date.
            
            If MyStocks.exists(strTicker) Then
                Set Stock = MyStocks(strTicker)
                Stock.Volume = Stock.Volume + Cells(i, 7).Value
                    ' Check to see if date is earlier than previously recorded, if so records open value.
                    ' Makes assumption that date on sheet is only from the listed year.
                    If dtDay < Stock.OpenDate Then
                        Stock.OpenDate = dtDay
                        Stock.OpenVal = Cells(i, 3).Value
                    End If
                    'Check to see if date is later and updates close val to this value.
                    If dtDay > Stock.CloseDate Then
                        Stock.CloseDate = dtDay
                        Stock.CloseVal = Cells(i, 6).Value
                    End If
            Else
                'Creates new stock ticker key for the dictionary.
                'Assumes if no opening value and no volume stock is not in business.
                If Not Cells(i, 3).Value = 0 And Cells(i, 7).Value = 0 Then
                    Set Stock = New clsStock
                    Stock.Ticker = strTicker
                    Stock.Volume = Cells(i, 7).Value
                    Stock.OpenDate = dtDay
                    Stock.CloseDate = dtDay
                    Stock.OpenVal = Cells(i, 3).Value
                    Stock.CloseVal = Cells(i, 6).Value
                    MyStocks.Add strTicker, Stock
                End If
            End If
            
        i = i + 1
    Loop
     j = 2
    'Output for annual summary
    For Each varKey In MyStocks.Keys
        Set Stock = MyStocks(varKey)
        If Not Stock.Volume = 0 Then
            sh.Range("I" & j).Value = Stock.Ticker
            lgChange = Stock.CloseVal - Stock.OpenVal
            sh.Range("J" & j).Value = lgChange
            If lgChange = 0 Then
                sh.Range("J" & j).Interior.ColorIndex = 0 'White
            ElseIf lgChange < 0 Then
                sh.Range("J" & j).Interior.ColorIndex = 3 'Red
            ElseIf lgChange > 0 Then
                sh.Range("J" & j).Interior.ColorIndex = 4 'Green
            End If
            sh.Range("J" & j).NumberFormat = "0.000"
            sh.Range("K" & j).Value = lgChange / Stock.OpenVal
            sh.Range("K" & j).NumberFormat = "0.00%"
            sh.Range("L" & j).Value = Stock.Volume
            j = j + 1
        End If
    Next
    
        'Summary chart, not creating an object but by using excel eqns
    Summarychart
    
        'Output preps the sheet with the headers to cover the summary information.
    Outputheaders
Next
End Sub

Sub Outputheaders()
	'Creates Headers for all columns 
  Range("I1") = "Ticker"
  Range("J1") = "Yearly Change"
  Range("K1") = "Percent Change"
  Range("L1") = "Total Stock Volumne"
  Range("I1:L1").Interior.ColorIndex = 0 'White
      
  Range("P1") = "Ticker"
  Range("Q1") = "Value"
  Range("P1:Q1").Interior.ColorIndex = 0 'White

End Sub

Sub Summarychart()
	'Creates summary chart
    Range("O2").Value = "Greatest % Increase"
    Range("O3").Value = "Greatest % Decrease"
    Range("O4").Value = "Greatest Total Volume"
	'Creates values
    Range("Q2").Formula = "=MAX(K:K)"
    Range("Q2").NumberFormat = "0.00%"
    Range("Q3").Formula = "=MIN(K:K)"
    Range("Q3").NumberFormat = "0.00%"
    Range("Q4").Formula = "=Max(L:L)"
    Range("Q4").NumberFormat = "0"
	'Finds related ticker value
    Range("P2").Formula = "=INDEX(I:I,MATCH(Q2,K:K,0),0)"
    Range("P3").Formula = "=INDEX(I:I,MATCH(Q2,K:K,0),0)"
    Range("P4").Formula = "=INDEX(I:I,MATCH(Q4,L:L,0),0)"
    
End Sub

'***************************
'******Class modules

'clsStock
Public Ticker As String
Public OpenDate As Date
Public CloseDate As Date
Public Volume As Double
Public OpenVal As Double
Public CloseVal As Double


