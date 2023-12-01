// 1abc2
// pqr3stu8vwx
// a1b2c3d4e5f
// treb7uchet

// 12
// 38
// 15
// 77

// two1nine
// eightwothree
// abcone2threexyz
// xtwone3four
// 4nineeightseven2
// zoneight234
// 7pqrstsixteen

// 29
// 83
// 13
// 24
// 42
// 14
// 76

package main

import (
	"bufio"
	"fmt"
	"os"
	"regexp"
	"strconv"
	"strings"
)

func main() {
	lines, err := readLines("./DataInput.txt")
	if err != nil {
		fmt.Println("Error opening file:", err)
		return
	}

	sum1 := 0
	sum2 := 0
	for _, line := range lines {
		str := line
		firstDigit1, lastDigit1 := findPart1Digits(str)
		firstDigit2, lastDigit2 := findPart2Digits(str)

		if err != nil {
			fmt.Println("Error converting digits to int:", err)
			continue
		}

		firstNumber1 := firstDigit1 * 10
		firstNumber2 := firstDigit2 * 10
		sum1 += firstNumber1 + lastDigit1
		sum2 += firstNumber2 + lastDigit2
	}

	fmt.Println("Answer1:", sum1)
	fmt.Println("Answer2:", sum2)
}

func findPart1Digits(str string) (int, int) {
	re := regexp.MustCompile(`\d`)
	matches := re.FindAllString(str, -1)
	return wordToDigit(matches[0]), wordToDigit(matches[len(matches)-1])
}

func findPart2Digits(str string) (int, int) {
	mappedDigits := map[string]string{"one": "1", "two": "2", "three": "3", "four": "4", "five": "5", "six": "6", "seven": "7", "eight": "8", "nine": "9", "1": "1", "2": "2", "3": "3", "4": "4", "5": "5", "6": "6", "7": "7", "8": "8", "9": "9"}

	firstDigit, lastDigit := "", ""
	newLine := ""

	// First digit
	for _, l := range str {
		newLine += string(l)

		for key := range mappedDigits {
			if strings.Contains(newLine, key) {
				firstDigit = key
				break
			}
		}

		if len(firstDigit) > 0 {
			break
		}
	}

	//Last digit
	newLine = ""

	for i := len(str) - 1; i >= 0; i-- {
		letter := string(str[i])

		newLine = letter + newLine
		for key := range mappedDigits {
			if strings.Contains(newLine, key) {
				lastDigit = key
				break
			}
		}

		if len(lastDigit) > 0 {
			break
		}
	}

	firstDigitInt, _ := strconv.Atoi(mappedDigits[firstDigit])
	lastDigitInt, _ := strconv.Atoi(mappedDigits[lastDigit])

	return firstDigitInt, lastDigitInt
}

func readLines(path string) ([]string, error) {
	file, _ := os.Open(path)
	defer file.Close()

	var lines []string
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		lines = append(lines, scanner.Text()) // append each line to the lines slice
	}
	return lines, scanner.Err()
}

func wordToDigit(text string) int {
	switch text {
	case "one":
		return 1
	case "two":
		return 2
	case "three":
		return 3
	case "four":
		return 4
	case "five":
		return 5
	case "six":
		return 6
	case "seven":
		return 7
	case "eight":
		return 8
	case "nine":
		return 9
	default:
		number, _ := strconv.Atoi(text)
		return number
	}
}
