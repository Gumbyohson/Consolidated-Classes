using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using conclass.Config;

namespace conclass.EntityBehaviors
{
 public class TemporalStabilityTraitBehavior : EntityBehavior
 {

  protected bool hasLocatedClass = false;
  protected float timeSinceLastUpdate = 0.0f;
  public bool hasClaustrophobia = false;
  public bool hasAgoraphobia = false;
  public bool hasShelteredStone = false;
  public bool hasDelver = false;
  public bool hasNone = true;
  public bool enabled;

  protected const double ShelteredByStoneGainVelocity = 0.002;
  protected const double DelverGainVelocity = -0.001;
  protected const int SunLightLevelForInCave = 5;

  public const string ClaustrophobiaCode = "claustrophobia";
  public const string AgoraphobiaCode = "agoraphobia";
  public const string ShelteredByStoneCode = "shelteredstone";
  public const string DelverCode = "delver";

  protected EntityBehaviorTemporalStabilityAffected? TemporalAffected => entity.GetBehavior<EntityBehaviorTemporalStabilityAffected>();

  public override string PropertyName() => "gcTemporalStabilityTraitBehavior";

  public TemporalStabilityTraitBehavior(Entity entity) : base(entity)
  {

  }

  public override void Initialize(EntityProperties properties, JsonObject attributes)
  {
   base.Initialize(properties, attributes);

   // Get the world's temporal stability setting (controlled by server)
   bool worldTemporalEnabled = entity.Api.World.Config.GetBool("temporalStability", true);
    
    // Get the server's mod config setting
    bool serverModEnabled = entity.Api.World.Config.GetBool("EnableTemporalStability", true);
    
    // Only enable if BOTH the world config AND the server's mod config allow it
    enabled = worldTemporalEnabled && serverModEnabled;
    
    entity.Api.Logger.VerboseDebug($"Temporal Stability System - World: {worldTemporalEnabled}, Server Mod: {serverModEnabled}, Final: {enabled}");
  }

  public override void OnGameTick(float deltaTime)
  {
   if (!enabled || entity == null || entity is not EntityPlayer)
   {
    return;
   }

   if (entity.World.PlayerByUid(((EntityPlayer)entity).PlayerUID) is IServerPlayer serverPlayer && serverPlayer.ConnectionState != EnumClientState.Playing)
   {
    return;
   }

   if (!hasNone)
   {
    HandleTraits(deltaTime);
   }

   if (hasLocatedClass)
   {
    return;
   }

   timeSinceLastUpdate += deltaTime;

   if (timeSinceLastUpdate > 1.5f) // Adjusted to tick every 1.5 seconds
   {
    timeSinceLastUpdate = 0.0f;

    if (!hasLocatedClass)
    {
     string? classcode = entity.WatchedAttributes.GetString("characterClass");
     CharacterClass? charclass = entity.Api.ModLoader.GetModSystem<CharacterSystem>().characterClasses.FirstOrDefault(c => c.Code == classcode);
     if (charclass != null)
     {
      if (charclass.Traits.Contains(ClaustrophobiaCode))
      {
       hasClaustrophobia = true;
      }
      if (charclass.Traits.Contains(AgoraphobiaCode))
      {
       hasAgoraphobia = true;
      }
      if (charclass.Traits.Contains(ShelteredByStoneCode))
      {
       hasShelteredStone = true;
      }
      if (charclass.Traits.Contains(DelverCode))
      {
       hasDelver = true;
      }

      if (hasClaustrophobia || hasAgoraphobia || hasDelver || hasShelteredStone)
      {
       hasNone = false; // This just might make the check a TINY bit quicker if it's only comparing a single bool for every 1.5s tick after this.
      }
      hasLocatedClass = true;
     }
    }
   }
  }

  public void HandleTraits(float deltaTime)
  {
   BlockPos pos = entity.Pos.AsBlockPos;
   var tempStabVelocity = TemporalAffected.TempStabChangeVelocity;

   if (hasShelteredStone)
   {
    if (entity.World.BlockAccessor.GetLightLevel(pos, EnumLightLevelType.OnlySunLight) < SunLightLevelForInCave)
    {
     if (tempStabVelocity > ShelteredByStoneGainVelocity)
     {
      return;
     }
     else
     {
      TemporalAffected.TempStabChangeVelocity = ShelteredByStoneGainVelocity;
      return;
     }
    }
   }

   if (hasAgoraphobia)
   {
    var room = entity.Api.ModLoader.GetModSystem<RoomRegistry>().GetRoomForPosition(pos);

    if (room == null || !(room.ExitCount == 0 || room.SkylightCount < room.NonSkylightCount))
    {
     var surfaceLoss = (double)entity.Stats.GetBlended("surfaceStabilityLoss") - 1; // The -1 should return the raw value.
     if (tempStabVelocity < surfaceLoss)
     {
      surfaceLoss = tempStabVelocity;
     }
     TemporalAffected.TempStabChangeVelocity = surfaceLoss;
     return;
    }
   }

   if (hasClaustrophobia)
   {
    if (entity.World.BlockAccessor.GetLightLevel(pos, EnumLightLevelType.OnlySunLight) < SunLightLevelForInCave && tempStabVelocity < 0)
    {
     var caveLoss = entity.Stats.GetBlended("caveStabilityLoss");
     TemporalAffected.TempStabChangeVelocity = (tempStabVelocity * caveLoss);
     return;
    }
   }

   if (hasDelver)
   {
    if (entity.World.BlockAccessor.GetLightLevel(pos, EnumLightLevelType.OnlySunLight) < SunLightLevelForInCave && tempStabVelocity < 0)
    {
     var caveLoss = entity.Stats.GetBlended("delverdeepStabilityLoss");
     TemporalAffected.TempStabChangeVelocity = tempStabVelocity * caveLoss;
     return;
    }
   }
  }
 }
}