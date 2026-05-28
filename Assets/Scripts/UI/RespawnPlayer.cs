using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    public bool RespawnMainCharacter(CharacterMovements mainCharacter, Transform respawnLocationCube)
    {

        Health healthComp = mainCharacter.GetComponent<Health>();


        CharacterController controller = mainCharacter.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        mainCharacter.transform.SetPositionAndRotation(respawnLocationCube.position, respawnLocationCube.rotation);

        if (controller != null)
        {
            controller.enabled = true;
        }

        CharacterMotor motor = mainCharacter.GetComponent<CharacterMotor>();
        if (motor != null)
        {
            motor.ResetMotion();
        }

        float missing = healthComp.MaxHealth - healthComp.CurrentHealth;
        if (missing > 0f)
        {
            healthComp.Heal(missing);
        }

        return true;
    }
}
