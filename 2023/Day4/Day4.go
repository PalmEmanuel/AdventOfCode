package main

// Importing necessary packages
import (
	"bufio"
	"os"
	"strconv"
	"strings"
)

func main() {
	// Reading lines from command-line argument file
	lines, _ := readLines(os.Args[1])

	totalScore := 0
	cards := make([]ParsedCard, len(lines))

	// Parsing each line and calculating the total score
	for i, line := range lines {
		card := parseCard(line)
		cards[i] = card
		totalScore += card.Score
	}

	println("Part 1 score: ", totalScore)

	totalCards := len(cards)
	queue := append([]ParsedCard(nil), cards...)

	// Calculating the total number of cards
	for len(queue) > 0 {
		card := queue[0]
		queue = queue[1:]
		matches := calculateMatches(card.WinningNumbers, card.CardNumbers)
		if card.CardNumber+matches <= len(cards) {
			totalCards += matches
			queue = append(queue, cards[card.CardNumber:card.CardNumber+matches]...)
		}
	}

	println("Part 2 total cards: ", totalCards)
}

// Function to calculate the number of matches between winning numbers and card numbers
func calculateMatches(winningNumbers []int, cardNumbers []int) int {
	matches := 0
	for _, num := range cardNumbers {
		for _, winNum := range winningNumbers {
			if num == winNum {
				matches++
				break
			}
		}
	}
	return matches
}

// Struct to represent a parsed card
type ParsedCard struct {
	CardNumber     int
	WinningNumbers []int
	CardNumbers    []int
	Score          int
}

// Function to parse a card string and return a ParsedCard struct
func parseCard(card string) ParsedCard {
	parts := strings.Split(card, "|")
	cardAndWinningNumbers := strings.Split(strings.TrimSpace(parts[0]), ":")
	cardNumber, _ := strconv.Atoi(strings.TrimSpace(cardAndWinningNumbers[0][5:])) // "Card X: ..." -> X

	// Parsing winning numbers
	winningNumbersStr := strings.Fields(cardAndWinningNumbers[1])
	winningNumbers := make([]int, len(winningNumbersStr))
	for i, numStr := range winningNumbersStr {
		num, _ := strconv.Atoi(numStr)
		winningNumbers[i] = num
	}

	// Parsing card numbers
	cardNumbersStr := strings.Fields(parts[1])
	cardNumbers := make([]int, len(cardNumbersStr))
	for i, numStr := range cardNumbersStr {
		num, _ := strconv.Atoi(numStr)
		cardNumbers[i] = num
	}

	// Calculating the score of the card
	score := calculateScore(winningNumbers, cardNumbers)

	return ParsedCard{
		CardNumber:     cardNumber,
		WinningNumbers: winningNumbers,
		CardNumbers:    cardNumbers,
		Score:          score,
	}
}

// Function to calculate the score of a card based on the number of matches
func calculateScore(winningNumbers []int, cardNumbers []int) int {
	score := 0
	matches := 0
	for _, num := range cardNumbers {
		for _, winNum := range winningNumbers {
			if num == winNum {
				matches++
				if matches == 1 {
					score = 1
				} else {
					score *= 2
				}
				break
			}
		}
	}
	return score
}

// Function to read lines from a file
func readLines(path string) ([]string, error) {
	file, err := os.Open(path)
	if err != nil {
		return nil, err
	}
	defer file.Close()

	var lines []string
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		lines = append(lines, scanner.Text())
	}
	return lines, scanner.Err()
}
