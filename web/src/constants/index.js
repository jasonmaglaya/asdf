export const EventStatus = {
  New: "New",
  Active: "Active",
  Closed: "Closed",
  Final: "Final",
};

export const MatchStatus = {
  New: "New",
  Open: "Open",
  Closed: "Closed",
  Finalized: "Finalized", //Fighting
  Declared: "Declared",
  Completed: "Completed",
  Cancelled: "Cancelled",
};

export const EventHubEvents = {
  JoinEvent: "JoinEvent",
  TotalBetsReceived: "TotalBetsReceived",
  MatchStatusReceived: "MatchStatusReceived",
  WinnerDeclared: "WinnerDeclared",
  WinnerReDeclared: "WinnerReDeclared",
  NextMatchReceived: "NextMatchReceived",
  RefreshVideo: "RefreshVideo",
  RefreshUsers: "RefreshUsers",
};

export const Features = {
  ControlMatch: "ControlMatch",
  ReDeclare: "ReDeclare",
  PropagateCredits: "PropagateCredits",
  UpdateRole: "UpdateRole",
  ActivateUser: "ActivateUser",
  ActivateAgency: "ActivateAgency",
  TopUp: "TopUp",
  TransferCredits: "TransferCredits",
  DeleteUser: "DeleteUser",
  MaintainEvents: "MaintainEvents",
  ListAllUsers: "ListAllUsers",
  Reports: "Reports",
  PreviewEvent: "PreviewEvent",
  ChangeOwnPassword: "ChangeOwnPassword",
};

export const AgentRoles = ["ag", "ma", "sma", "inco"];
export const AdminRoles = ["admin", "sa"];
export const SuperAdminRoles = ["sa"];
export const PlayerRoles = ["player"];

export const Roles = {
  player: "Player",
  ag: "Agent",
  ma: "MA",
  sma: "SMA",
  inco: "Incorporator",
  admin: "Admin",
  sa: "Super Admin",
};

export const UserRoles = {
  Player: "player",
  Agent: "ag",
  MA: "ma",
  SMA: "sma",
  Inco: "inco",
  Admin: "admin",
  SuperAdmin: "sa",
};

export const DownLines = {
  ma: "Agent",
  sma: "MA",
  inco: "SMA",
  admin: "INCO",
  sa: "INCO",
};

export const pathFeatureMappings = {
  "/events": Features.MaintainEvents,
  "/events/": Features.MaintainEvents,
  "/events/new": Features.MaintainEvents,
  "/events/new/": Features.MaintainEvents,
  "/users": Features.ListAllUsers,
  "/users/": Features.ListAllUsers,
  "/reports": Features.Reports,
  "/reports/": Features.Reports,
};
