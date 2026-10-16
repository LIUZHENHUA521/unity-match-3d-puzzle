  (func $j3031 (type 29) (param i32 i32 f32 i32 i32) (result i32)
    (local i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 5
    local.get 5
    global.set 0
    i32.const 4501539
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3418920
      call $j870 ;; 
      i32.const 3418960
      call $j870 ;; 
      i32.const 3430336
      call $j870 ;; 
      i32.const 3502472
      call $j870 ;; 
      i32.const 3516180
      call $j870 ;; 
      i32.const 3516184
      call $j870 ;; 
      i32.const 3444760
      call $j870 ;; 
      i32.const 4501539
      i32.const 1
      i32.store8
    end
    i32.const 3444760
    i32.load
    call $j873 ;; 
    local.set 4
    local.get 4
    local.get 0
    i32.store offset=8
    i32.const 3418920
    i32.load
    call $j873 ;; 
    local.set 0
    local.get 0
    local.get 4
    i32.const 3516180
    i32.load
    call $j1131 ;; 
    i32.const 3418960
    i32.load
    call $j873 ;; 
    local.set 6
    local.get 6
    local.get 4
    i32.const 3516184
    i32.load
    call $j1130 ;; 
    i32.const 3430336
    i32.load
    call $j871 ;; 
    local.get 5
    local.get 1
    i32.load offset=8
    i32.store offset=8
    local.get 1
    i32.load offset=4
    local.set 7
    local.get 5
    local.get 1
    i32.load
    i32.store
    local.get 5
    local.get 7
    i32.store offset=4
    local.get 0
    local.get 6
    local.get 5
    local.get 2
    local.get 4
    call $j1874 ;; DOTween.To
    local.set 1
    local.get 1
    local.set 0
    block  ;; label = @1
      local.get 0
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=144
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      local.get 3
      i32.store8 offset=236
    end
    local.get 0
    local.get 4
    i32.load offset=8
    i32.const 3502472
    i32.load
    call $j1157 ;; 
    drop
    local.get 5
    i32.const 16
    i32.add
    global.set 0
    local.get 1
    return)
