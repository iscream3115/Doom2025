using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		public bool shoot;

		public bool switchFist;
		public bool switchPistol;
		public bool switchShotgun;
		public bool switchSSG;
		public bool switchRocket;
		public bool switchPlasma;
		public bool switchChaingun;
		public bool switchChainsaw;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}

		public void OnSwitchFist(InputValue value)
		{
			SwitchFistInput(value.isPressed);
		}

		public void OnSwitchPistol(InputValue value)
		{
			SwitchPistolInput(value.isPressed);
		}

		public void OnSwitchShotgun(InputValue value)
		{
			SwitchShotgunInput(value.isPressed);
		}

		public void OnSwitchSSG(InputValue value)
		{
			SwitchSSGInput(value.isPressed);
		}

		public void OnSwitchRocket(InputValue value)
		{
			SwitchRocketInput(value.isPressed);
		}

		public void OnSwitchPlasma(InputValue value)
		{
			SwitchPlasmaInput(value.isPressed);
		}
		public void OnSwitchChaingun(InputValue value)
		{
			SwitchChaingunInput(value.isPressed);
		}
		public void OnSwitchChainsaw(InputValue value)
		{
			SwitchChainsawInput(value.isPressed);
		}

#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}

		public void SwitchFistInput(bool newSwitchState)
		{
			switchFist = newSwitchState;
		}

		public void SwitchPistolInput(bool newSwitchState)
		{
			switchPistol = newSwitchState;
		}

		public void SwitchShotgunInput(bool newSwitchState)
		{
			switchShotgun = newSwitchState;
		}

		public void SwitchSSGInput(bool newSwitchState)
		{
			switchSSG = newSwitchState;
		}
		public void SwitchRocketInput(bool newSwitchState)
		{
			switchRocket = newSwitchState;
		}
		public void SwitchPlasmaInput(bool newSwitchState)
		{
			switchPlasma = newSwitchState;
		}
		public void SwitchChaingunInput(bool newSwitchState)
		{
			switchChaingun = newSwitchState;
		}
		public void SwitchChainsawInput(bool newSwitchState)
		{
			switchChainsaw = newSwitchState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}