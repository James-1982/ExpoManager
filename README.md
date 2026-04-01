# ExpoManager

# login using default admin account:
- email: admin@expo.com
- pwd: Admin123!
(from swagger => login => get token => set Bearer + token on Authorization button).

# Admin Can register new user using regitration endpoint => new user will have role of Supervisor 
  Supervisor: can do all but it can not delete entity from db (only admin can)

# Admin can promote or demote any other user account

#---------------------------------------------------#
1. ENTITIES

  a. Pavilion: must have a name. 
     Others properties are:
     - Area
     - PoweredBy
     - Description
     - ImagePath
     - LastModify
     - ModifyBy
     - A list of Tag
     - A list of related Stand

  b. ExhibitionArea (Sector): must have a name. 
     Others properties are:
     - Type
     - State
     - IsHighlighted
     - ImagePath
     - LastModify
     - ModifyBy
     - A list of Tag
     - A list of related Stand

  c. Stand: must have a name. 
     Others properties are:
     - Dimensions
     - PavilionId (associated Pavilion)
     - ExhibitionAreaId (associated ExhibitionArea)
     - ImagePath
     - LastModify
     - ModifyBy
     - A list of Tag
     - A list of related Stand

  d. Category: must have a name. 
     Others properties are:
     - IsHighlighted
     - ImagePath
     - LastModify
     - ModifyBy
     - A list of Tag
     - A list of related Stand

  e. Tag: must have a name. (internal entity)

//TODO: HOw relate Categoy with others entities
  
     
