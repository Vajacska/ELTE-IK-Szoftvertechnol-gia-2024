using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class GameField : ViewModelBase
    {
        private bool _isPlayer1;
        private bool _isPlayer1GhostFull;
        private bool _isPlayer1GhostLow;
        private bool _isPlayer1InvincibleFull;
        private bool _isPlayer1InvincibleLow;
        private bool _isPlayer2;
        private bool _isPlayer2GhostFull;
        private bool _isPlayer2GhostLow;
        private bool _isPlayer2InvincibleFull;
        private bool _isPlayer2InvincibleLow;
        private bool _isPlayer3;
        private bool _isPlayer3GhostFull;
        private bool _isPlayer3GhostLow;
        private bool _isPlayer3InvincibleFull;
        private bool _isPlayer3InvincibleLow;

        private bool _isWall;
        private bool _isBomb;
        private bool _isDangerZone;

        private bool _isExtraBombs;
        private bool _isBiggerBlast;
        private bool _isDetonator;
        private bool _isRollerskate;
        private bool _isInvincibility;
        private bool _isGhostPowerup;
        private bool _isBarrier;
        private bool _isSlower;
        private bool _isSmallRange;
        private bool _isNoBomb;
        private bool _isInstantBomb;

        private bool _isBox;

        private bool _isDefault;

        private bool _isGhost;

        private bool _isFrankeinstein;

        private bool _isNoob;

        public bool IsBox
        {
            get { return _isBox; }
            set
            {
                if (_isBox != value)
                {
                    _isBox = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool IsPlayer1
        {
            get { return _isPlayer1; }
            set
            {
                if (_isPlayer1 != value)
                {
                    _isPlayer1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer1GhostFull
        {
            get { return _isPlayer1GhostFull; }
            set
            {
                if (_isPlayer1GhostFull != value)
                {
                    _isPlayer1GhostFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer1GhostLow
        {
            get { return _isPlayer1GhostLow; }
            set
            {
                if (_isPlayer1GhostLow != value)
                {
                    _isPlayer1GhostLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer1InvincibleFull
        {
            get { return _isPlayer1InvincibleFull; }
            set
            {
                if (_isPlayer1InvincibleFull != value)
                {
                    _isPlayer1InvincibleFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer1InvincibleLow
        {
            get { return _isPlayer1InvincibleLow; }
            set
            {
                if (_isPlayer1InvincibleLow != value)
                {
                    _isPlayer1InvincibleLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer2
        {
            get { return _isPlayer2; }
            set
            {
                if (_isPlayer2 != value)
                {
                    _isPlayer2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer2GhostFull
        {
            get { return _isPlayer2GhostFull; }
            set
            {
                if (_isPlayer2GhostFull != value)
                {
                    _isPlayer2GhostFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer2GhostLow
        {
            get { return _isPlayer2GhostLow; }
            set
            {
                if (_isPlayer2GhostLow != value)
                {
                    _isPlayer2GhostLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer2InvincibleFull
        {
            get { return _isPlayer2InvincibleFull; }
            set
            {
                if (_isPlayer2InvincibleFull != value)
                {
                    _isPlayer2InvincibleFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer2InvincibleLow
        {
            get { return _isPlayer2InvincibleLow; }
            set
            {
                if (_isPlayer2InvincibleLow != value)
                {
                    _isPlayer2InvincibleLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer3
        {
            get { return _isPlayer3; }
            set
            {
                if (_isPlayer3 != value)
                {
                    _isPlayer3 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer3GhostFull
        {
            get { return _isPlayer3GhostFull; }
            set
            {
                if (_isPlayer3GhostFull != value)
                {
                    _isPlayer3GhostFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer3GhostLow
        {
            get { return _isPlayer3GhostLow; }
            set
            {
                if (_isPlayer3GhostLow != value)
                {
                    _isPlayer3GhostLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer3InvincibleFull
        {
            get { return _isPlayer3InvincibleFull; }
            set
            {
                if (_isPlayer3InvincibleFull != value)
                {
                    _isPlayer3InvincibleFull = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayer3InvincibleLow
        {
            get { return _isPlayer3InvincibleLow; }
            set
            {
                if (_isPlayer3InvincibleLow != value)
                {
                    _isPlayer3InvincibleLow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsExtraBombs
        {
            get { return _isExtraBombs; }
            set
            {
                if (_isExtraBombs != value)
                {
                    _isExtraBombs = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBiggerBlast
        {
            get { return _isBiggerBlast; }
            set
            {
                if (_isBiggerBlast != value)
                {
                    _isBiggerBlast = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDetonator
        {
            get { return _isDetonator; }
            set
            {
                if (_isDetonator != value)
                {
                    _isDetonator = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRollerSkate
        {
            get { return _isRollerskate; }
            set
            {
                if (_isRollerskate != value)
                {
                    _isRollerskate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsInvincibility
        {
            get { return _isInvincibility; }
            set
            {
                if (_isInvincibility != value)
                {
                    _isInvincibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGhostPowerup
        {
            get { return _isGhostPowerup; }
            set
            {
                if (_isGhostPowerup != value)
                {
                    _isGhostPowerup = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBarrier
        {
            get { return _isBarrier; }
            set
            {
                if (_isBarrier != value)
                {
                    _isBarrier = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSlower
        {
            get { return _isSlower; }
            set
            {
                if (_isSlower != value)
                {
                    _isSlower = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSmallRange
        {
            get { return _isSmallRange; }
            set
            {
                if (_isSmallRange != value)
                {
                    _isSmallRange = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNoBomb
        {
            get { return _isNoBomb; }
            set
            {
                if (_isNoBomb != value)
                {
                    _isNoBomb = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsInstantBomb
        {
            get { return _isInstantBomb; }
            set
            {
                if (_isInstantBomb != value)
                {
                    _isInstantBomb = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsWall
        {
            get { return _isWall; }
            set
            {
                if (_isWall != value)
                {
                    _isWall = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBomb
        {
            get { return _isBomb; }
            set
            {
                if (_isBomb != value)
                {
                    _isBomb = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDangerZone
        {
            get { return _isDangerZone; }
            set
            {
                if (_isDangerZone != value)
                {
                    _isDangerZone = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool IsDefault
        {
            get { return _isDefault; }
            set
            {
                if (_isDefault != value)
                {
                    _isDefault = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGhost
        {
            get { return _isGhost; }
            set
            {
                if (_isGhost != value)
                {
                    _isGhost = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsFrankeinstein
        {
            get { return _isFrankeinstein; }
            set
            {
                if (_isFrankeinstein != value)
                {
                    _isFrankeinstein = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNoob
        {
            get { return _isNoob; }
            set
            {
                if (_isNoob != value)
                {
                    _isNoob = value;
                    OnPropertyChanged();
                }
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public Point XY { get { return new Point(X, Y); } }
    }
}
