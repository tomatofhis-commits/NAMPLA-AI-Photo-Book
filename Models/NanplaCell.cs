using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Nanpla.Models
{
    public class NanplaCell : BindableObject
    {
        private int _value;
        private bool _isError;
        private bool _isOriginal;
        private bool _isSelected;
        private bool[] _notes = new bool[9];

        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public ObservableCollection<int> Notes { get; } = new ObservableCollection<int>();

        public bool Note1 => _notes[0];
        public bool Note2 => _notes[1];
        public bool Note3 => _notes[2];
        public bool Note4 => _notes[3];
        public bool Note5 => _notes[4];
        public bool Note6 => _notes[5];
        public bool Note7 => _notes[6];
        public bool Note8 => _notes[7];
        public bool Note9 => _notes[8];

        public bool IsError
        {
            get => _isError;
            set
            {
                if (_isError != value)
                {
                    _isError = value;
                    OnPropertyChanged(nameof(IsError));
                }
            }
        }

        public bool IsOriginal
        {
            get => _isOriginal;
            set
            {
                if (_isOriginal != value)
                {
                    _isOriginal = value;
                    OnPropertyChanged(nameof(IsOriginal));
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsEmpty => Value == 0;

        public bool HasNotes => _notes.Any(n => n);

        public NanplaCell()
        {
        }

        public bool GetNote(int num)
        {
            if (num < 1 || num > 9) return false;
            return _notes[num - 1];
        }

        public void SetNote(int num, bool val)
        {
            if (num < 1 || num > 9) return;
            if (_notes[num - 1] != val)
            {
                _notes[num - 1] = val;
                OnPropertyChanged($"Note{num}");
                OnPropertyChanged(nameof(HasNotes));
            }
        }

        public void AddNote(int note)
        {
            SetNote(note, true);
        }

        public void RemoveNote(int note)
        {
            SetNote(note, false);
        }

        public void ClearNotes()
        {
            for (int i = 1; i <= 9; i++)
            {
                SetNote(i, false);
            }
        }
    }
}
