const int WinningLength = 5;
const int BoardSize = 19;

int step = 1;

// Directions: horizontal, vertical, diagonal (top-left to bottom-right), diagonal (top-right to bottom-left)
int[][] Directions = new int[][]
{
        [ 0, 1 ], // horizontal
        [ 1, 0 ], // vertical
        [ -1, 1 ], // diagonal (bottom-left to top-right)
        [ 1, 1 ] // diagonal (top-left to bottom-right)
};

bool IsWinningMove(int[,] board, int x, int y, int player)
{
    foreach (var dir in Directions)
    {
        int count = 1; // Count the current stone
        // Check in the positive direction
        for (step = 1; step < WinningLength; ++step)
        {
            int nx = x + dir[0] * step;
            int ny = y + dir[1] * step;
            if (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize && board[nx, ny] == player)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count >= WinningLength)
        {
            return true;
        }
    }
    return false;
}

static int[,] ParseRenjuBoard(string[] lines)
{
    var board = new int[BoardSize, BoardSize];

    if (lines.Length != BoardSize)
    {
        throw new FormatException($"Expected {BoardSize} lines of board data, but found {lines.Length}.");
    }

    for (int i = 0; i < BoardSize; ++i)
    {
        var values = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != BoardSize)
        {
            throw new FormatException($"Expected {BoardSize} columns of board data, but found {values.Length}.");
        }

        int[] valuesInt = values.Select(int.Parse).ToArray();

        if (valuesInt.Any(v => v < 0 || v > 2))
        {
            throw new FormatException($"Line {i} contains invalid values. Only 0, 1, and 2 are allowed.");
        }

        for (int j = 0; j < BoardSize; ++j)
        {
            board[i, j] = valuesInt[j];
        }
    }

    return board;
}

static void ValidateBoard(int[,] board)
{
    var countBlack = board.Cast<int>().Count(v => v == (int)Stone.Black);
    var countWhite = board.Cast<int>().Count(v => v == (int)Stone.White);

    if (countBlack != countWhite && countBlack != countWhite + 1)
    {
        throw new FormatException($"Invalid board state. Black has {countBlack} stones and White has {countWhite} stones.");
    }
}

var content = await File.ReadAllLinesAsync("input.txt");

if (content.Length == 0)
{
    Console.WriteLine("Empty file.");
    return;
}

if (!int.TryParse(content[0], out var tests))
{
    Console.WriteLine("Invalid number of test cases.");
    return;
}

if ((content.Length - 1) % BoardSize != 0)
{
    Console.WriteLine($"Invalid board test data. Each board in test case should consist of {BoardSize} lines.");
    return;
}

int currentIndex = 1;

using var output = new StreamWriter("output.txt", append: false);

while (tests-- > 0)
{
    var currentContent = content[currentIndex..(currentIndex + BoardSize)];
    currentIndex += BoardSize;

    int[,] board = ParseRenjuBoard(currentContent);

    try
    {
        board = ParseRenjuBoard(currentContent);
        ValidateBoard(board);
    }
    catch (Exception ex)
    {
        await output.WriteLineAsync("Invalid test case skipped. " + ex.Message);
        continue;
    }

    bool anyWon = false;

    for (int i = 0; i < BoardSize && !anyWon; ++i)
    {
        for (int j = 0; j < BoardSize; ++j)
        {
            var stone = board[i, j];
            if (stone != (int)Stone.Empty && IsWinningMove(board, i, j, stone))
            {
                await output.WriteLineAsync(stone.ToString());
                await output.WriteLineAsync($"{i + 1} {j + 1}");
                anyWon = true;
                break;
            }
        }
    }

    if (!anyWon)
    {
        await output.WriteLineAsync("0");
        await output.FlushAsync();
    }
}

await output.FlushAsync();
output.Close();

enum Stone
{
    Empty,
    Black,
    White
}