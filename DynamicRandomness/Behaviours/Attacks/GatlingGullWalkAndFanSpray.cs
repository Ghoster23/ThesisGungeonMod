using UnityEngine;

namespace DynamicRandomness.Behaviours.Attacks
{
	public class GatlingGullWalkAndFanSpray : BasicAttackBehavior
	{
		private float m_durationTimer;

		private float m_remainingDuration;

		private float m_totalDuration;


		public float Duration;

		public float AngleVariance;

		public bool ContinuesOnPathComplete;

		public string OverrideBulletName;

		public float SprayAngle;

		public float SpraySpeed;

		public float SprayIterations;

		public GatlingGullWalkAndFanSpray() : base()
		{
			this.Duration = 5f;
			this.AngleVariance = 20f;

			this.SprayAngle = 120f;
			this.SpraySpeed = 60f;
			this.SprayIterations = 4;
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Upkeep()
		{
			base.Upkeep();
			base.DecrementTimer(ref this.m_durationTimer, false);
		}

		public override BehaviorResult Update()
		{
			base.Update();
			if (!this.m_aiActor.TargetRigidbody)
			{
				return BehaviorResult.Continue;
			}

			this.m_aiActor.SuppressTargetSwitch = true;

			this.m_aiShooter.volley.projectiles[0].angleVariance = 0f;
			AkSoundEngine.PostEvent("Play_ANM_Gull_Shoot_01", this.m_gameObject);
			this.m_totalDuration = this.SprayAngle / this.SpraySpeed * (float)this.SprayIterations;
			this.m_remainingDuration = this.m_totalDuration;

			AkSoundEngine.PostEvent("Play_ANM_Gull_Loop_01", this.m_gameObject);
			AkSoundEngine.PostEvent("Play_ANM_Gull_Gatling_01", this.m_gameObject);

			return BehaviorResult.RunContinuousInClass;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();

			if (this.ContinuesOnPathComplete)
			{
				this.m_aiAnimator.OverrideIdleAnimation = "idle_shoot";
			}

			if (this.m_durationTimer <= 0f || !this.m_aiActor.TargetRigidbody || (this.m_aiActor.PathComplete && !this.ContinuesOnPathComplete))
			{
				return ContinuousBehaviorResult.Finished;
			}

			float num = 1f - this.m_remainingDuration / this.m_totalDuration;
			float num2 = num * (float)this.SprayIterations % 2f;
			float num3 = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiShooter.volleyShootPosition.position.XY()).ToAngle();
			num3 = BraveMathCollege.QuantizeFloat(num3, 45f);
			num3 += -this.SprayAngle / 2f + Mathf.PingPong(num2 * this.SprayAngle, this.SprayAngle);
			this.m_aiShooter.ShootInDirection(Quaternion.Euler(0f, 0f, num3) * Vector2.right, this.OverrideBulletName);

			return ContinuousBehaviorResult.Continue;
		}

		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			if (this.ContinuesOnPathComplete)
			{
				this.m_aiAnimator.OverrideIdleAnimation = string.Empty;
			}
			this.m_aiShooter.ManualGunAngle = false;
			this.UpdateCooldowns();
			this.m_aiActor.SuppressTargetSwitch = false;
			AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.m_aiActor.GetComponent<AkGameObj>())
			{
				AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
			}
		}
	}
}
