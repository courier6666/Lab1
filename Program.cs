const int WinningLength = 5;
const int BoardSize = 19;

static bool IsWinningMove(int[,] board, int x, int y, int player)
{
    int N = board.GetLength(0);
    int M = board.GetLength(1);
    // Directions: horizontal, vertical, diagonal (top-left to bottom-right), diagonal (top-right to bottom-left)
    int[][] directions = new int[][]
    {
        [ 0, 1 ], // horizontal
        [ 1, 0 ], // vertical
        [ -1, 1 ], // diagonal (bottom-left to top-right)
        [ 1, 1 ] // diagonal (top-left to bottom-right)
    };
    foreach (var dir in directions)
    {
        int count = 1; // Count the current stone
        // Check in the positive direction
        for (int step = 1; step < WinningLength; ++step)
        {
            int nx = x + dir[0] * step;
            int ny = y + dir[1] * step;
            if (nx >= 0 && nx < N && ny >= 0 && ny < M && board[nx, ny] == player)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            return true;
        }
    }
    return false;
}

static int[,] ParseRenjuBoard(string[] lines)
{
    int N = BoardSize;
    int M = BoardSize;

    var board = new int[N, M];

    if (lines.Length != N)
    {
        throw new FormatException($"Expected {N} lines of board data, but found {lines.Length}.");
    }

    for (int i = 0; i < N; ++i)
    {
        var values = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != M)
        {
            throw new FormatException($"Expected {M} columns of board data, but found {values.Length}.");
        }

        int[] valuesInt = values.Select(int.Parse).ToArray();

        if (valuesInt.Any(v => v < 0 || v > 2))
        {
            throw new FormatException($"Line {i} contains invalid values. Only 0, 1, and 2 are allowed.");
        }

        for (int j = 0; j < M; ++j)
        {
            board[i, j] = valuesInt[j];
        }
    }

    return board;
}

var content = await File.ReadAllLinesAsync("input.txt");

var tests = int.Parse(content[0]);
int currentIndex = 1;

using var output = new StreamWriter("output.txt", append: false);

while (tests-- > 0)
{
    var currentContent = content[currentIndex..(currentIndex + BoardSize)];
    currentIndex += BoardSize;

    var board = ParseRenjuBoard(currentContent);

    bool anyWon = false;

    for (int i = 0; i < BoardSize; ++i)
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

        if (anyWon)
        {
            break;
        }
    }

    if (anyWon)
    {
        continue;
    }

    await output.WriteLineAsync("0");
    await output.FlushAsync();
}

await output.FlushAsync();
output.Close();

enum Stone
{
    Empty,
    Black,
    White
}