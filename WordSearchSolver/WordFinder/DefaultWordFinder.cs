﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WordSearchSolverTests")]
namespace WordSearchSolver
{
    internal class DefaultWordFinder : IWordFinder
    {
        private Word _word;
        private IList<Coordinate> _location;
        private char[,] _puzzle;

        public DefaultWordFinder()
        {
            _puzzle = new char[,] { };
        }

        public void LoadPuzzle(char[,] puzzle)
        {
            _puzzle = puzzle;
        }

        public void FindWord(Word word)
        {
            _word = word;
            _location = new List<Coordinate>();
            var shouldStopSearching = false;
            var wordFound = false;
            var wordFirstLetter = _word.Text[0];

            int row = 0;
            while (!shouldStopSearching)
            {
                for (int column = 0; column < _puzzle.GetLength(1); column++)
                {
                    var current_puzzleLetter = _puzzle[row, column];
                    if (current_puzzleLetter != wordFirstLetter)
                        continue;

                    wordFound = SearchForWord(row, column);

                    if (wordFound)
                    {
                        shouldStopSearching = true;
                        break;
                    }
                }
                row++;
                if (row == _puzzle.GetLength(0))
                    shouldStopSearching = true;
            }
            _word.Location = wordFound ? _location : null;
        }

        private bool SearchForWord(int row, int column)
        {
            var trySearchMethods = new List<SearchMethodDelegate>
            {
                SearchDown,
                SearchUp,
                SearchBackward,
                SearchForward,
                SearchDownAndForward,
                SearchUpAndBackward,
                SearchDownAndBackward,
                SearchUpAndForward
            };

            foreach (var method in trySearchMethods)
            {
                var wordFound = method(row, column);
                if (wordFound)
                {
                    return wordFound;
                }
            }
            return false;
        }

        private delegate bool SearchMethodDelegate(int row, int column);

        #region Search Methods

        private bool SearchDown(int row, int column)
        {
            if (WordOutOfBoundsDown(row))
                return false;

            return FindLetter((i) => row + i, (i) => column);
        }

        private bool SearchUp(int row, int column)
        {
            if (WordOutOfBoundsUp(row))
                return false;

            return FindLetter((i) => row - i, (i) => column);
        }

        private bool SearchBackward(int row, int column)
        {
            if (WordOutOfBoundsBackward(column))
                return false;

            return FindLetter((i) => row, (i) => column - i);
        }

        private bool SearchForward(int row, int column)
        {
            if (WordOutOfBoundsForward(column))
                return false;

            return FindLetter((i) => row, (i) => column + i);
        }

        private bool SearchDownAndForward(int row, int column)
        {
            if (WordOutOfBoundsDown(row) || WordOutOfBoundsForward(column))
                return false;

            return FindLetter((i) => row + i, (i) => column + i);
        }

        private bool SearchUpAndBackward(int row, int column)
        {
            if (WordOutOfBoundsUp(row) || WordOutOfBoundsBackward(column))
                return false;

            return FindLetter((i) => row - i, (i) => column - i);
        }

        private bool SearchDownAndBackward(int row, int column)
        {
            if (WordOutOfBoundsDown(row) || WordOutOfBoundsBackward(column))
                return false;

            return FindLetter((i) => row + i, (i) => column - i);
        }

        private bool SearchUpAndForward(int row, int column)
        {
            if (WordOutOfBoundsUp(row) || WordOutOfBoundsForward(column))
                return false;

            return FindLetter((i) => row - i, (i) => column + i);
        }

        #endregion

        private bool WordOutOfBoundsDown(int row)
        {
            return row + _word.Text.Length > _puzzle.GetLength(1);
        }

        private bool WordOutOfBoundsForward(int column)
        {
            return _word.Text.Length + column > _puzzle.GetLength(0);
        }

        private bool WordOutOfBoundsUp(int row)
        {
            return row - _word.Text.Length + 1 < 0;
        }

        private bool WordOutOfBoundsBackward(int column)
        {
            return column - _word.Text.Length + 1 < 0;
        }

        private void UpdateLocation(int row, int column)
        {
            _location.Add(new Coordinate(row, column));
        }

        private bool LetterFound(int index, int row, int column)
        {
            if (_puzzle[row, column] == _word.Text[index])
            {
                return true;
            }
            return false;
        }

        private bool FindLetter(Func<int, int> rowStepper, Func<int, int> columnStepper)
        {
            for (int i = 0; i < _word.Text.Length; i++)
            {
                var row = rowStepper(i);
                var column = columnStepper(i);

                var letterFound = LetterFound(i, row, column);

                if(letterFound)
                {
                    UpdateLocation(row, column);
                    continue;
                }
                else
                {
                    ResetLocation();
                    return false;
                }
            }
            return true;
        }

        private void ResetLocation()
        {
            _location = new List<Coordinate>();
        }
    }
}
