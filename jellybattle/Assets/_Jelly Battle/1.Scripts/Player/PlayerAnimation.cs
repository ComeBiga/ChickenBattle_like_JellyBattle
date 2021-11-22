using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private PhotonView photonView;

    private void Start()
    {
        anim = GetComponent<Animator>();
        photonView = PhotonView.Get(this);
    }

    [PunRPC]
    private void AnimateRPC(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void Animate(string name, bool value)
    {
        AnimateRPC(name, value);
        photonView.RPC("AnimateRPC", RpcTarget.Others, name, value);
    }

    public void AnimateLocal(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void PlayerRun()
    {
        anim.SetBool("IsRun", true);
    }

    [PunRPC]
    private void IdleAnimationRPC()
    {
        anim.SetBool("IsRun", false);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("Hit", false);
        anim.SetBool("IsDead", false);
        anim.SetBool("IsBoneAttacking", false);
        anim.SetBool("IsBomb", false);
        anim.SetBool("IsBoltAttacking", false);
        anim.SetBool("IsSniperAttacking", false);
        anim.SetBool("IsIced", false);
    }

    public void IdleAnimation()
    {
        IdleAnimationRPC();
        photonView.RPC("IdleAnimationRPC", RpcTarget.Others);
    }

    public void IdleAnimationLocal()
    {
        anim.SetBool("IsRun", false);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("Hit", false);
        anim.SetBool("IsDead", false);
        anim.SetBool("IsBoneAttacking", false);
        anim.SetBool("IsBomb", false);
        anim.SetBool("IsBoltAttacking", false);
        anim.SetBool("IsSniperAttacking", false);
        anim.SetBool("IsIced", false);
    }

    [PunRPC]
    private void AttackAnimationRPC()
    {
        anim.SetBool("IsAttacking", true);
    }

    public void AttackAnimation()
    {
        AttackAnimationRPC();
        photonView.RPC("AttackAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void AttackBoneAnimationRPC()
    {
        anim.SetBool("IsBoneAttacking", true);
    }

    public void AttackBoneAnimation()
    {
        AttackBoneAnimationRPC();
        photonView.RPC("AttackBoneAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void AttackBoltAnimationRPC()
    {
        anim.SetBool("IsBoltAttacking", true);
    }

    public void AttackBoltAnimation()
    {
        AttackBoltAnimationRPC();
        photonView.RPC("AttackBoltAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void AttackSniperAnimationRPC()
    {
        anim.SetBool("IsSniperAttacking", true);
    }

    public void AttackSniperAnimation()
    {
        AttackSniperAnimationRPC();
        photonView.RPC("AttackSniperAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void BombAnimationRPC()
    {
        anim.SetBool("IsBomb", true);
    }
    
    public void BombAnimation()
    {
        BombAnimationRPC();
        photonView.RPC("BombAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void HitAnimationRPC()
    {
        Debug.Log("HitAnimationRPC / " + Time.time);
        anim.SetBool("Hit", true);
    }

    public void HitAnimation()
    {
        Debug.Log("HitAnimation / " + Time.time);
        HitAnimationRPC();
        photonView.RPC("HitAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void DeadAnimationRPC()
    {
        anim.SetBool("IsDead", true);
    }

    public void DeadAnimation()
    {
        DeadAnimationRPC();
        photonView.RPC("DeadAnimationRPC", RpcTarget.Others);
    }

    [PunRPC]
    private void IcedAnimationRPC()
    {
        anim.SetBool("IsIced", true);
    }

    public void IcedAnimation()
    {
        IcedAnimationRPC();
        photonView.RPC("IcedAnimationRPC", RpcTarget.Others);
    }
}
