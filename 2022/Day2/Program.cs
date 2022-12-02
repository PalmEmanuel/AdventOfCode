var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

// Example input
// A Y
// B X
// C Z
int totalScore = 0;
foreach (var line in input)
{
    int score = 0;
    char opponentPlay = line[0];
    char responsePlay = line[2];

    score += GetOutcomeScore(opponentPlay, responsePlay);
    score += GetPlayScore(responsePlay);

    totalScore += score;
}

Console.WriteLine(totalScore);

totalScore = 0;
foreach (var line in input)
{
    int score = 0;
    char opponentPlay = line[0];
    char responsePlay = line[2];

    score += GetOutcomeScorePart2(opponentPlay, responsePlay);
    responsePlay = TranslateResponsePlay(opponentPlay, responsePlay);
    score += GetPlayScore(responsePlay);

    totalScore += score;
}

Console.WriteLine(totalScore);

static int GetPlayScore(char input)
{
    switch (input)
    {
        case 'X':
            return 1;
        case 'Y':
            return 2;
        case 'Z':
            return 3;
        default:
            return 0;
    }
}

// Brute force cause it's morning and I'm tired
static int GetOutcomeScore(char opponent, char response)
{
    if (opponent == 'A')
    {
        if (response == 'X')
        {
            return 3;
        }
        if (response == 'Y')
        {
            return 6;
        }
        if (response == 'Z')
        {
            return 0;
        }
    }
    if (opponent == 'B')
    {
        if (response == 'X')
        {
            return 0;
        }
        if (response == 'Y')
        {
            return 3;
        }
        if (response == 'Z')
        {
            return 6;
        }
    }
    if (opponent == 'C')
    {
        if (response == 'X')
        {
            return 6;
        }
        if (response == 'Y')
        {
            return 0;
        }
        if (response == 'Z')
        {
            return 3;
        }
    }

    return 0;
}

static int GetOutcomeScorePart2(char opponent, char response)
{
    if (response == 'X')
    {
        return 0;
    }
    if (response == 'Y')
    {
        return 3;
    }
    if (response == 'Z')
    {
        return 6;
    }

    return 0;
}

static char TranslateResponsePlay(char opponent, char response)
{
    if (opponent == 'A')
    {
        if (response == 'X')
        {
            return 'Z';
        }
        if (response == 'Y')
        {
            return 'X';
        }
        if (response == 'Z')
        {
            return 'Y';
        }
    }
    if (opponent == 'B')
    {
        if (response == 'X')
        {
            return 'X';
        }
        if (response == 'Y')
        {
            return 'Y';
        }
        if (response == 'Z')
        {
            return 'Z';
        }
    }
    if (opponent == 'C')
    {
        if (response == 'X')
        {
            return 'Y';
        }
        if (response == 'Y')
        {
            return 'Z';
        }
        if (response == 'Z')
        {
            return 'X';
        }
    }

    throw new Exception("Not valid input!");
}