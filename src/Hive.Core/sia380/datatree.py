# ONLY FOR DEBUGGING

class DataTree:
    
    def __init__(self, data):
        self._branches = [Branch(d) for d in data]
    
    @property
    def BranchCount(self):
        return len(self._branches)
    
    @property
    def Branches(self):
        return self._branches
    
    @property
    def DataCount(self):
        pass
    
    @property
    def Item(self):
        pass
    
    @Item.setter
    def Item(self, i):
        pass
    
    @property
    def Paths(self):
        pass
    
    def Branch(self, i):        
        assert i < len(self._branches), "Invalid index"
        return self._branches[i]
    
    @property
    def Path(self, i):
        pass
    
    def AddBranch(self, branch):
        self._branches.append(branch)
        return len(self._branches-1)

class Branch(list):
    
    def __init__(self, data):
        super(Branch, self).__init__(data)
    
    @property
    def Count(self):
        return self.count
    

    
    # a = []

    # for i in range(x.BranchCount):
    #     branchList = x.Branch(i)
    #     branchPath = x.Path(i)
        
    #     for j in range(branchList.Count):
    #         s = str(branchPath) + "[" + str(j) + "] "
    #         s += type(branchList[j]).__name__ + ": "
    #         s += str(branchList[j])
            
    #         a.append(s)